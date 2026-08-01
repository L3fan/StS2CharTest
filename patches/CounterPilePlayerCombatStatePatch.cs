using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using StS2CharTest.CustomNodes;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(PlayerCombatState), MethodType.Constructor, new Type[] {typeof(Player)})]
internal class CounterPilePlayerCombatStatePatch
{
    [HarmonyPostfix]
    public static void Postfix(PlayerCombatState __instance)
    {
        CounterPileResource.PlayerCombatStateCounterPile.Set(__instance, new CounterPile());
    }
}