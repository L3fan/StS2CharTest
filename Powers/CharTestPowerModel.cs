
using BaseLib.Abstracts;
using BaseLib.Extensions;
using StS2CharTest.Cards;

namespace StS2CharTest.Powers;

public abstract class CharTestPowerModel : CustomPowerModel, CharTestModel
{
    public override string CustomPackedIconPath => "res://images/sts2chartest/powers/" + Id.Entry.RemovePrefix().ToLowerInvariant() + ".png";
    public override string CustomBigIconPath => "res://images/sts2chartest/powers/big/" + Id.Entry.RemovePrefix().ToLowerInvariant() + ".png";

    public virtual Task OnBlazeTriggered()
    {
        return Task.CompletedTask;
    }
}