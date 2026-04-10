using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using StS2CharTest.Cards;
using StS2CharTest.Code.Powers;

namespace StS2CharTest.Actions;

public static class CharTestActions
{
    public static async Task GainHeat(Creature target, decimal amount)
    {
        if (!target.IsPlayer || amount <= 0)
            return;
        Player player = target.Player;
        decimal currentHeat = HeatResource.Amount.Get(player.PlayerCombatState);
        decimal setToAmount = currentHeat + amount;

        if (currentHeat + amount > HeatResource.MaxAmount.Get(player.PlayerCombatState))
        {
            setToAmount -= HeatResource.MaxAmount.Get(player.PlayerCombatState);
            await CommonActions.Apply<OverheatPower>(target, null, 1);
        }
        
        HeatResource.Amount.Set(player.PlayerCombatState, setToAmount);
        Action<int, int> heatChanged = HeatResource.HeatChanged.Get(player.PlayerCombatState);
        heatChanged.Invoke((int)currentHeat, (int)setToAmount);

        CombatState combatState = target.CombatState;
        if (target.CombatState == null)
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
}