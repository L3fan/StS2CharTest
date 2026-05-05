using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace StS2CharTest.Powers;

public class DragonScalesPower : CharTestPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterHeatGained(PlayerChoiceContext choiceContext, int amount, Player gainer)
    {
        await CommonActions.Apply<VigorPower>(choiceContext, Owner, null, amount);
    }
}