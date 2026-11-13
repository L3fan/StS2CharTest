using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using StS2CharTest.Code.Character;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyHpLost))]
internal class RageDamageBlockPatch
{
    private static void Postfix(Creature target, HpLossHookPhase phases, ref Decimal __result)
    {
        if (!target.IsPlayer)
            return;

        if (phases != HpLossHookPhase.AfterOsty)
            return;
        
        Player player = target.Player;
        Decimal rageAmount = RageResource.Amount.Get(player.PlayerCombatState);
        
        MainFile.Logger.Info("Player taking " + __result + " Damage");
        
        Decimal rageLeft = Math.Max(rageAmount - __result, 0);
        Decimal remainingDamage = Math.Max(__result - rageAmount, 0);
        
        __result = remainingDamage;
        
        RageResource.Amount.Set(player.PlayerCombatState, rageLeft);
        RageResource.RageChanged.Get(player.PlayerCombatState)?.Invoke((int)rageAmount, (int)rageLeft);
    }
}