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
        new EmbersVar(5m), 
        new BlazeVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.Apply<EmbersPower>(choiceContext, play.Target, this, DynamicVars["EmbersPower"].IntValue);
        await Blaze(Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["EmbersPower"].UpgradeValueBy(3m);
    }
}