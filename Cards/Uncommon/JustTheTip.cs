using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using StS2CharTest.Code.Character;
using StS2CharTest.Powers;

namespace StS2CharTest.Cards.Uncommon;

[Pool(typeof(CharTestCardPool))]
public class JustTheTip() : CharTestCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1), new PowerVar<JustTheTipPower>(1).WithTooltip("HEAT")];
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.Draw(this, choiceContext);
        await CommonActions.ApplySelf<JustTheTipPower>(this, DynamicVars["JustTheTipPower"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["JustTheTipPower"].UpgradeValueBy(1m);
    }
}