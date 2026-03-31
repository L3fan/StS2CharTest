using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using StS2CharTest.Code.Character;
using StS2CharTest.Code.Powers;

namespace StS2CharTest.Cards.Common;

[Pool(typeof(CharTestCardPool))]
public class CoolOff() : CharTestCard(1, CardType.Skill,
    CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<HeatPower>(1).WithTooltip("HEAT"), new CardsVar(3)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        HeatPower heatPower = Owner.Creature.GetPower<HeatPower>();
        if (heatPower != null)
            await PowerCmd.ModifyAmount(heatPower, -DynamicVars["HeatPower"].IntValue, Owner.Creature, this);
        await CommonActions.Draw(this, choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2);
    }
}