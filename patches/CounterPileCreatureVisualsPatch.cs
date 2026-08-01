using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(NCreatureVisuals))]
internal class CounterPileCreatureVisualsPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(NCreatureVisuals._Ready))]
    public static void Postfix(NCreatureVisuals __instance)
    {
        MainFile.Logger.Info("Adding Counter Pile Node");
        string counterPileScenePath = "res://scenes/sts2chartest/counter_pile.tscn";
        NCounterPile nCounterPile = GD.Load<PackedScene>(counterPileScenePath).Instantiate<NCounterPile>();
        CounterPileResource.CreatureVisualsCounterPile.Set(__instance, nCounterPile);
        __instance.AddChild(nCounterPile);
        nCounterPile.Position = new Vector2(250, -250);
    }
}