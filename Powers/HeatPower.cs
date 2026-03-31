using System.Buffers;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using StS2CharTest.Powers;

namespace StS2CharTest.Code.Powers;

public class HeatPower : CharTestPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("MaxStack", 20)];

    private bool _shouldTriggerOnHeatChanged = true;
    private decimal _excessAmount = 0;
    private decimal _amountChange = 0;

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier,
        out decimal modifiedAmount)
    {
        _shouldTriggerOnHeatChanged = true;
        _amountChange = amount;
        _excessAmount = 0;
        MainFile.Logger.Info("Amount gained: " + _amountChange + " // Amount already existing: " + Amount + " // Max Stack: " + DynamicVars["MaxStack"].IntValue + " // " + (_amountChange + Amount >= DynamicVars["MaxStack"].IntValue));
        if (_amountChange + Amount >= DynamicVars["MaxStack"].IntValue)
        {
            _excessAmount = Amount + _amountChange - DynamicVars["MaxStack"].IntValue;
            _amountChange = DynamicVars["MaxStack"].IntValue - Amount;
            _shouldTriggerOnHeatChanged = false;
        }
        modifiedAmount = _amountChange;

        return true;
    }

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (_shouldTriggerOnHeatChanged)
        {
            foreach (PowerModel p in Owner.Powers)
            {
                if (p.GetType() != typeof(IHeatChangeActor))
                    continue;
                IHeatChangeActor actor = p as IHeatChangeActor;
                actor.OnHeatChanged((int)(_amountChange + _excessAmount));
            }
        }
        else
            _shouldTriggerOnHeatChanged = true;
        
        if (Amount < DynamicVars["MaxStack"].IntValue)
            return;
        _shouldTriggerOnHeatChanged = false;
        await PowerCmd.ModifyAmount(this, _excessAmount - Amount, Owner, null);
        await PowerCmd.Apply<OverheatPower>(Owner, 1, Owner, null);
    }

    public interface IHeatChangeActor
    {
        Task OnHeatChanged(int amount);
    }
}