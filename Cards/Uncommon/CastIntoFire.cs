using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using StS2CharTest.Actions;
using StS2CharTest.Code.Character;

namespace StS2CharTest.Cards.Uncommon;

[Pool(typeof(CharTestCardPool))]
public class CastIntoFire() : CharTestCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new ExhaustiveVar(3)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars["Exhaustive"].IntValue);
        foreach (CardModel card in await CardSelectCmd.FromHand(choiceContext, Owner, prefs, (Func<CardModel, bool>)null, (AbstractModel)this))
        {
            await CardCmd.Exhaust(choiceContext, card);
            await CardPileCmd.Draw(choiceContext, 1, Owner);
            await CharTestActions.GainHeat(Owner, 1);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}