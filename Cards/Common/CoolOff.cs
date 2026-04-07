using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using StS2CharTest.Actions;
using StS2CharTest.Code.Character;
using StS2CharTest.Code.Powers;

namespace StS2CharTest.Cards.Common;

[Pool(typeof(CharTestCardPool))]
public class CoolOff() : CharTestCard(1, CardType.Skill,
    CardRarity.Common, TargetType.Self)
{
    public override int CanonicalHeatCost => 1;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(9m, ValueProp.Move).WithTooltip("HEAT_COUNT")];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}