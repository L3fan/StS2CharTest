using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using StS2CharTest.Code.Character;
using StS2CharTest.Code.Powers;

namespace StS2CharTest.Cards.Common;

[Pool(typeof(CharTestCardPool))]
public class WarmUpStrike() : CharTestCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move), new PowerVar<HeatPower>(2).WithTooltip("HEAT")];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        await CommonActions.ApplySelf<HeatPower>(this, DynamicVars["HeatPower"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HeatPower"].UpgradeValueBy(1);
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}