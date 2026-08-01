using System.Net;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using StS2CharTest.CustomNodes;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(CardPile))]
internal class CounterPileCardPilePatch
{
    [HarmonyPrefix, HarmonyPatch("Get")]
    public static bool Prefix(ref CardPile __result, ref PileType type, ref Player player)
    {
        if (type == CounterPile.Counter)
        {
            __result = CounterPileResource.PlayerCombatStateCounterPile.Get(player.PlayerCombatState);
            return false;
        }

        return true;
    }
}