using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using StS2CharTest.CustomNodes;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(PileTypeExtensions))]
internal class CounterPileTypeExtensionsPatch
{
    [HarmonyPrefix, HarmonyPatch(nameof(PileTypeExtensions.IsCombatPile))]
    public static bool Prefix(ref PileType pileType, ref bool __result)
    {
        if (pileType == CounterPile.Counter)
        {
            __result = true;
            return false;
        }

        return true;
    }
}