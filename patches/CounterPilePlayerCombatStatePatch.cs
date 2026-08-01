using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(PlayerCombatState))]
internal class CounterPilePlayerCombatStatePatch
{
    [HarmonyPostfix, HarmonyPatch("PlayerCombatState", MethodType.Constructor)]
    public static void Postfix(PlayerCombatState __instance, ref Player __player)
    {
        CounterPileResource.PlayerCombatStateCounterPile.Set(__instance, new CardPile(CounterPileResource.Counter));
    }
}