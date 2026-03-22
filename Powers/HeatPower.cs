using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using StS2CharTest.Powers;

namespace StS2CharTest.Code.Powers;

public class HeatPower : CharTestPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        decimal newAmount = amount;
        if (newAmount + Amount > 20)
        {
            newAmount += Amount - 20;
            await PowerCmd.Apply<OverheatPower>(Owner, newAmount, Owner, null);
        }
        
        await base.BeforeApplied(target, newAmount, applier, cardSource);
    }
}