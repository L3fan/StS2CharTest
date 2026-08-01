using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2CharTest.Cards;
using StS2CharTest.Code.Powers;
using StS2CharTest.Powers;

namespace StS2CharTest.Actions;

public static class CharTestActions
{
    public static async Task GainHeat(PlayerChoiceContext choiceContext, Creature target, int amount)
    {
        if (!target.IsPlayer)
            return;
        await GainHeat(choiceContext, target.Player, amount);
    }
    public static async Task GainHeat(PlayerChoiceContext choiceContext, Player target, decimal amount)
    {
        if (amount <= 0)
            return;
        Player player = target;
        decimal currentHeat = HeatResource.Amount.Get(player.PlayerCombatState);
        decimal setToAmount = currentHeat + amount;

        if (currentHeat + amount > HeatResource.MaxAmount.Get(player.PlayerCombatState))
        {
            setToAmount -= HeatResource.MaxAmount.Get(player.PlayerCombatState);
            await CommonActions.Apply<OverheatPower>(player.Creature, null, 1);
        }
        
        HeatResource.Amount.Set(player.PlayerCombatState, setToAmount);
        Action<int, int> heatChanged = HeatResource.HeatChanged.Get(player.PlayerCombatState);
        heatChanged.Invoke((int)currentHeat, (int)setToAmount);

        ICombatState combatState = target.Creature.CombatState;
        if (combatState == null)
            return;
        foreach (AbstractModel model in combatState.IterateHookListeners())
        {
            MainFile.Logger.Info("Is " + model.GetType() + " is CharTestModel? " + (model is CharTestModel));
            if (model is CharTestModel)
            {
                CharTestModel charTestModel = model as CharTestModel;
                await charTestModel.AfterHeatGained(choiceContext, (int)amount, player);
            }
        }
    }

    public static async Task SpendHeat(Creature target, decimal amount)
    {
        if (!target.IsPlayer || amount <= 0)
            return;
        Player player = target.Player;
        decimal currentHeat = HeatResource.Amount.Get(player.PlayerCombatState);
        decimal setToAmount = (decimal)Mathf.Max((float)(currentHeat - amount), 0);
        
        HeatResource.Amount.Set(player.PlayerCombatState, setToAmount);
        Action<int, int> heatChanged = HeatResource.HeatChanged.Get(player.PlayerCombatState);
        heatChanged.Invoke((int)currentHeat, (int)setToAmount);
    }
    
    public static async Task Blaze(ICombatState combatState, Player source, int triggerAmount = 1, bool triggerEffects = true)
    {
        await TriggerEmbers(combatState, source.Creature, triggerAmount, triggerEffects);
    }

    public static async Task Blaze(ICombatState combatState, Creature source, int triggerAmount = 1, bool triggerEffects = true)
    {
        await TriggerEmbers(combatState, source, triggerAmount, triggerEffects);
    }

    public static async Task TriggerEmbers(ICombatState combatState, Creature source, int triggerAmount = 1, bool triggerEffects = true)
    {
        for (int i = 0; i < triggerAmount; i++)
        {
            foreach (AbstractModel iterateHookListener in combatState.IterateHookListeners())
            {
                if (iterateHookListener is not EmbersPower)
                    continue;
                EmbersPower embers = iterateHookListener as EmbersPower;
                Creature owner = embers.Owner;
                if (!owner.IsDead)
                {
                    if (owner.IsAlive)
                    {
                        await embers.TriggerBlazeDamage(source, triggerEffects);
                    }
                }
            }
        }
    }
}