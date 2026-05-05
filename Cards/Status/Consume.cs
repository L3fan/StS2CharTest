using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using StS2CharTest.Actions;
using StS2CharTest.Powers;

namespace StS2CharTest.Cards.Status;

[Pool(typeof(StatusCardPool))]
public class Consume() : CharTestCard(0, CardType.Status,
    CardRarity.Status, TargetType.Self)
{
    public override int MaxUpgradeLevel => 0;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>(1).WithTooltip("STRENGTH"), new CardsVar(1)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<StrengthPower>(choiceContext, this);
        await CommonActions.Draw(this, choiceContext);
    }
}