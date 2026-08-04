using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(NCombatPilesContainer))]
internal class NCombatPilesContainerCounterPilePatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(NCombatPilesContainer.Initialize))]
    public static void PostFix(ref Player player)
    {
        NCreature playerNode = NCombatRoom.Instance.GetCreatureNode(player.Creature);
        CounterPileResource.NCreatureVisualsNCounterPile.Get(playerNode.Visuals).Initialize(player);
    }
}