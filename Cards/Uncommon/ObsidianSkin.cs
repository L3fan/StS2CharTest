using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using StS2CharTest.Code.Character;
using StS2CharTest.Powers;

namespace StS2CharTest.Cards.Uncommon;

[Pool(typeof(CharTestCardPool))]
public class ObsidianSkin() : CharTestCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ObsidianSkinPower>(4).WithTooltip("HEAT")];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<ObsidianSkinPower>(this, DynamicVars["ObsidianSkinPower"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ObsidianSkinPower"].UpgradeValueBy(2);
    }
}