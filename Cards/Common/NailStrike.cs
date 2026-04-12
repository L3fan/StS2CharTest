using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using StS2CharTest.Code.Character;

namespace StS2CharTest.Cards.Common;

[Pool(typeof(CharTestCardPool))]
public class NailStrike() : CharTestCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move), new RepeatVar(2)];

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike, CharTest.Nails];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play, DynamicVars.Repeat.IntValue).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}