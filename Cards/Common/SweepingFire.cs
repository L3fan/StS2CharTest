using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using StS2CharTest.Code.Character;
using StS2CharTest.Powers;

namespace StS2CharTest.Cards.Common;

[Pool(typeof(CharTestCardPool))]
public class SweepingFire() : CharTestCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move), new PowerVar<EmbersPower>(4m).WithTooltip("EMBERS")
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        MainFile.Logger.Info("Targets: " + play.Target);
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            await CommonActions.Apply<EmbersPower>(enemy, this, DynamicVars["EmbersPower"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["EmbersPower"].UpgradeValueBy(3m);
    }
}