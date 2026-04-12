using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using StS2CharTest.Code.Character;
using StS2CharTest.Powers;

namespace StS2CharTest.Cards.Common;

[Pool(typeof(CharTestCardPool))]
public class Cauterize() : CharTestCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<EmbersPower>(5m).WithTooltip("EMBERS"), 
        new DynamicVar("Blaze", 1).WithTooltip("BLAZE")
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.Apply<EmbersPower>(play.Target, this, DynamicVars["EmbersPower"].IntValue);
        await TriggerBlaze();
    }

    protected override void OnUpgrade()
    {
        DynamicVars["EmbersPower"].UpgradeValueBy(3m);
    }
}