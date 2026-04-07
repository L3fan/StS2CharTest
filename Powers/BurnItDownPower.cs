using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace StS2CharTest.Powers;

public class BurnItDownPower : CharTestPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Type != CardType.Status)
            return;
        await CardCmd.Exhaust(choiceContext, card, false, true);
        await CommonActions.Draw(null, choiceContext);
        await DamageCmd.Attack(Amount).TargetingRandomOpponents(CombatState).WithHitFx("vfx/vfx_attack_blunt").Execute(choiceContext);
    }
}