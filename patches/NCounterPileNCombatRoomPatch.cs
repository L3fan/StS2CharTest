using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(NCombatRoom))]
internal class NCounterPileNCombatRoomPatch
{
    [HarmonyPostfix, HarmonyPatch("CreateAllyNodes")]
    public static void AddNCounterPile(ref NCombatRoom __instance, ref Control ____allyContainer, ref ICombatRoomVisuals ____visuals)
    {
        string nCounterPilePath = "res://scenes/sts2chartest/counter_pile.tscn";
        PackedScene nCounterPilePackedScene = GD.Load<PackedScene>(nCounterPilePath);

        if (____allyContainer.GetChildren() != null)
        {
            foreach (Node child in ____allyContainer.GetChildren())
            {
                MainFile.Logger.Info("Ally Container child: " + child.GetType());
                if (child is not NCreature)
                    continue;

                NCreature creature = child as NCreature;

                if (!creature.Entity.IsPlayer || creature.Entity.Player == null)
                    continue;

                NCounterPile nCounterPile = nCounterPilePackedScene.Instantiate<NCounterPile>();
                nCounterPile.Position = new Vector2(160, -175);
                creature.Visuals.AddChild(nCounterPile);
                Player player = LocalContext.GetMe(((CombatRoom)____visuals).CombatState);
                nCounterPile.Initialize(player);
                CounterPileResource.NCreatureVisualsNCounterPile.Set(creature.Visuals, nCounterPile);
            }
        }

    }
}