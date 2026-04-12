using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using StS2CharTest.Code.Character;
using StS2CharTest.Powers;

namespace StS2CharTest.Cards.Uncommon;

[Pool(typeof(CharTestCardPool))]
public class Heatwave() : CharTestCard(2, CardType.Skill,
    CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<EmbersPower>(5m).WithTooltip("EMBERS"), new BoolVar("empty", false).WithTooltip("BLAZE")];
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (Creature enemy in CombatState.HittableEnemies)
            await CommonActions.Apply<EmbersPower>(enemy, this, DynamicVars["EmbersPower"].IntValue);
        await TriggerBlaze();
    }

    protected override void OnUpgrade()
    {
        DynamicVars["EmbersPower"].UpgradeValueBy(3m);
    }
}