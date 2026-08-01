using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2CharTest.Actions;
using StS2CharTest.Cards;
using StS2CharTest.Code.Character;

namespace StS2CharTest.Code.Cards;

[Pool(typeof(CharTestCardPool))]
public class DefendCharTest() : CharTestCard(1,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (Pile == CardPile.Get(CounterPileResource.Counter, Owner))
        {
            await CommonActions.CardBlock(this, play);
        }
        else
            await CharTestActions.AddToCounterPile(new CardModel[] { this }, Owner.Creature);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}