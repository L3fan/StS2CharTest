using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(PileTypeExtensions))]
internal class CounterPileTypeExtensionsPatch
{
    [HarmonyPrefix, HarmonyPatch(nameof(PileTypeExtensions.IsCombatPile))]
    public static bool Prefix(ref PileType __pileType, ref bool __result)
    {
        if (__pileType == CounterPileResource.Counter)
        {
            __result = true;
            return false;
        }

        return true;
    }
}