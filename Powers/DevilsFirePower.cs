using MegaCrit.Sts2.Core.Entities.Powers;
using StS2CharTest.Actions;

namespace StS2CharTest.Powers;

public class DevilsFirePower : CharTestPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task OnBlaze(bool triggerEffects = true)
    {
        await CharTestActions.Blaze(CombatState, Owner, triggerEffects:false);
    }
}