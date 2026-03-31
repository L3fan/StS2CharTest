using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using StS2CharTest.Code.Powers;

namespace StS2CharTest.Powers;

public class ObsidianSkinPower : CharTestPowerModel, HeatPower.IHeatChangeActor
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if(side != Owner.Side)
            return;

        HeatPower heatPower = Owner.GetPower<HeatPower>();
        if (heatPower == null)
            return;

        await PowerCmd.Decrement(heatPower);
    }

    public async Task OnHeatChanged(int change)
    {
        if (change >= 0)
            return;
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}