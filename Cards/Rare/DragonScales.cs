using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using StS2CharTest.Code.Character;
using StS2CharTest.Powers;

namespace StS2CharTest.Cards.Rare;

[Pool(typeof(CharTestCardPool))]
public class DragonScales() : CharTestCard(1,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DragonScalesPower>(7).WithTooltip("VIGOR")];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<DragonScalesPower>(choiceContext, this, DynamicVars["DragonScalesPower"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DragonScalesPower"].UpgradeValueBy(3);
    }
}