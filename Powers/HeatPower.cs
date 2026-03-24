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

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        decimal newAmount = amount;
        if (newAmount + Amount > DynamicVars["MaxStack"].IntValue)
        {
            newAmount += Amount - DynamicVars["MaxStack"].IntValue;
            await PowerCmd.Apply<OverheatPower>(Owner, newAmount, Owner, null);
        }
        
        await base.BeforeApplied(target, newAmount, applier, cardSource);
    }
}