using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using StS2CharTest.Actions;
using StS2CharTest.Code.Character;
using StS2CharTest.Powers;

namespace StS2CharTest.Cards.Rare;

[Pool(typeof(CharTestCardPool))]
public class Firestorm() : CharTestCard(3, CardType.Attack,
    CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(16m, ValueProp.Move), new PowerVar<EmbersPower>(3), new BlazeVar(1)];

    public override int CanonicalHeatCost => 5;
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        foreach (Creature enemy in CombatState.HittableEnemies.ToList())
        {
            await CommonActions.Apply<EmbersPower>(choiceContext, enemy, this, DynamicVars["EmbersPower"].IntValue + LastHeatSpent);
        }

        await Blaze(Owner, DynamicVars["Blaze"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
        DynamicVars["Blaze"].UpgradeValueBy(1);
    }
}