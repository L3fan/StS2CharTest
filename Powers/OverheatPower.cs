using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using StS2CharTest.Powers;

namespace StS2CharTest.Code.Powers;

public class OverheatPower : CharTestPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        decimal newAmount = amount;
        if (newAmount + Amount > 3)
            return;
        await base.BeforeApplied(target, newAmount, applier, cardSource);
    }

    public override string CustomPackedIconPath => "res://images/sts2chartest/powers/" + Id.Entry.RemovePrefix().ToLowerInvariant() + ".png";
    public override string CustomBigIconPath => "res://images/sts2chartest/powers/big/" + Id.Entry.RemovePrefix().ToLowerInvariant() + ".png";
}