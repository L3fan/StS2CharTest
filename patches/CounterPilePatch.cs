using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(NCreature))]
public class CounterPilePatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(NCreatureVisuals._Ready))]
    public static void Postfix(NCreatureVisuals __instance)
    {
        string counterPileScenePath = "res://scenes/sts2chartest/counter_pile.tscn";
        NCounterPile nCounterPile = __instance.GetNode<NCounterPile>(counterPileScenePath);
        CounterPileResource._CounterPile.Set(__instance, nCounterPile);
        __instance.AddChild(nCounterPile);
        nCounterPile.Position = new Vector2(170, 250);
    }
}

public class CounterPileResource
{
    public static readonly SpireField<NCreatureVisuals, Node2D> _CounterPile = new(() => null);
}