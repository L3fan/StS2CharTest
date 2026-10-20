using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using StS2CharTest.Actions;
using StS2CharTest.Code.Character;

namespace StS2CharTest.Cards.Common;

[Pool(typeof(CharTestCardPool))]
public class Meal() : CharTestCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new RageVar(2m), new CardsVar(2)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CharTestActions.GainRage(choiceContext, Owner, DynamicVars["Heat"].IntValue);
        await CommonActions.Draw(this, choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Rage"].UpgradeValueBy(1);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}