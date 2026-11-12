using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using StS2CharTest.CustomNodes;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(PileTypeExtensions), nameof(PileTypeExtensions.IsCombatPile))]
internal class PileTypeExtensionsCounterPilePatch
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, PileType pileType)
    {
        if(pileType == CounterPile.Counter)
        {
            __result = true;
        }
    }
}