using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using StS2CharTest.Cards;
using StS2CharTest.Code.Powers;

namespace StS2CharTest.Actions;

public static class CharTestActions
{
    public static async Task GainHeat(Creature target, int amount)
    {
        if (!target.IsPlayer)
            return;
        await GainHeat(target.Player, amount);
    }
    public static async Task GainHeat(Player target, decimal amount)
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

        CombatState combatState = target.Creature.CombatState;
        if (combatState == null)
            return;
        foreach (AbstractModel model in combatState.IterateHookListeners())
        {
            if (model.GetType().IsSubclassOf(typeof(CharTestModel)))
            {
                CharTestModel charTestModel = model as CharTestModel;
                await charTestModel.AfterHeatGained((int)amount, player);
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
}