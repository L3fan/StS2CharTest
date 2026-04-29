
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Players;
using StS2CharTest.Cards;

namespace StS2CharTest.Powers;

public abstract class CharTestPowerModel : CustomPowerModel, CharTestModel
{
    public override string CustomPackedIconPath => "res://images/sts2chartest/powers/" + Id.Entry.RemovePrefix().ToLowerInvariant() + ".png";
    public override string CustomBigIconPath => "res://images/sts2chartest/powers/big/" + Id.Entry.RemovePrefix().ToLowerInvariant() + ".png";

    public virtual Task OnBlaze()
    {
        return Task.CompletedTask;
    }
    
    public virtual Task AfterHeatSpent(int amount, Player spender)
    {
        return Task.CompletedTask;
    }
    
    public virtual Task AfterHeatGained(int amount, Player gainer)
    {
        return Task.CompletedTask;
    }
}