
using BaseLib.Abstracts;
using BaseLib.Extensions;

namespace StS2CharTest.Powers;

public abstract class CharTestPowerModel : CustomPowerModel
{
    public override string CustomPackedIconPath => "res://images/sts2chartest/powers/" + Id.Entry.RemovePrefix().ToLowerInvariant() + ".png";
    public override string CustomBigIconPath => "res://images/sts2chartest/powers/big/" + Id.Entry.RemovePrefix().ToLowerInvariant() + ".png";

    public virtual Task OnBlazeTriggered(bool reduceEmbers = true)
    {
        return Task.CompletedTask;
    }
}