using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using StS2CharTest.Code.Character;
using StS2CharTest.Powers;

namespace StS2CharTest.Cards.Rare;

[Pool(typeof(CharTestCardPool))]
public class MeltingPoint() : CharTestCard(0, CardType.Skill,
    CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1).WithTooltip("EMBERS")];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.Draw(this, choiceContext);
        
        foreach(Creature target in CombatState.HittableEnemies)
            await CommonActions.Apply<MeltingPointPower>(target, this, 1);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}