using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;
using StS2CharTest.Cards;
using StS2CharTest.Code.Character;
using StS2CharTest.Powers;

namespace StS2CharTest.Code.Cards;

[Pool(typeof(CharTestCardPool))]
public class TailSwipe() : CharTestCard(1, CardType.Attack,
    CardRarity.Basic, TargetType.AnyEnemy)
{
    protected int embersInfliction = 6;
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new CalculationBaseVar(0m), 
        new ExtraDamageVar(1m), 
        new PowerVar<EmbersPower>(embersInfliction).WithTooltip("EMBERS"), 
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? target) => (target?.GetPowerAmount<EmbersPower>() ?? 0) + (target != null ? (target.GetPowerAmount<ArtifactPower>() > 0 ? 0 : (card as TailSwipe).embersInfliction) : 0))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.Apply<EmbersPower>(play.Target, this, DynamicVars["EmbersPower"].IntValue);
        await CommonActions.CardAttack(this, play.Target, (decimal)DynamicVars.CalculatedDamage.IntValue + play.Target.GetPowerAmount<EmbersPower>()).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["EmbersPower"].UpgradeValueBy(2m);
        embersInfliction = DynamicVars["EmbersPower"].IntValue;
    }

    protected override void AfterDowngraded()
    {
        embersInfliction = DynamicVars["EmbersPower"].IntValue;
    }

    public int GetEmbersInfliction(CardModel card, Creature? target)
    {
        MainFile.Logger.Info("Card: " + card + ", Target: " + target);
        return embersInfliction;
    }

    public int GetEmbersStacks(Creature target)
    {
        MainFile.Logger.Info("Embers stacks on target: " + (target?.GetPowerAmount<EmbersPower>() ?? 0));
        return target?.GetPowerAmount<EmbersPower>() ?? 0;
    }
}