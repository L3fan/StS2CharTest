using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using StS2CharTest.Actions;
using StS2CharTest.Code.Character;
using StS2CharTest.Powers;

namespace StS2CharTest.Cards.Common;

[Pool(typeof(CharTestCardPool))]
public class Cauterize() : CounterCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new RageVar(7m).WithTooltip("RAGE")
    ];

    protected override async Task OnCounterPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CharTestActions.GainRage(choiceContext, Owner, DynamicVars["Rage"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Rage"].UpgradeValueBy(2m);
    }
}