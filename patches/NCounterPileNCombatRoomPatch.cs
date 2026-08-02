using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(NCombatRoom))]
internal class NCounterPileNCombatRoomPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(NCombatRoom.AddCreature))]
    public static void AddNCounterPile(ref NCombatRoom __instance, ref Control ____allyContainer)
    {
        string nCounterPilePath = "res://scenes/sts2chartest/counter_pile.tscn";
        PackedScene nCounterPilePackedScene = GD.Load<PackedScene>(nCounterPilePath);

        if (____allyContainer.GetChildren() != null)
        {
            foreach (Node child in ____allyContainer.GetChildren())
            {
                if (child is not NCreature)
                    continue;

                NCreature creature = child as NCreature;

                if (!creature.Entity.IsPlayer || creature.Entity.Player == null)
                    continue;

                NCounterPile nCounterPile = nCounterPilePackedScene.Instantiate<NCounterPile>();
                nCounterPile.Initialize(creature.Entity.Player);
                creature.AddChild(nCounterPile);
            }
        }

    }
}