using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(NCombatPilesContainer))]
internal class NCounterPileNCombatRoomPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(NCombatPilesContainer._Ready))]
    public static void AddNCounterPile(ref NCombatPilesContainer __instance)
    {
        string nCounterPilePath = "res://scenes/sts2chartest/counter_pile.tscn";
        PackedScene nCounterPilePackedScene = GD.Load<PackedScene>(nCounterPilePath);

        NCounterPile nCounterPile = nCounterPilePackedScene.Instantiate<NCounterPile>();
        nCounterPile.Position = new Vector2(790, 500);
        __instance.AddChild(nCounterPile);
        //Player player = LocalContext.GetMe(((CombatRoom)____visuals).CombatState);
        //nCounterPile.Initialize(player);
        MainFile.Logger.Info("Created NCounterPile!");

    }
}