using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using StS2CharTest.CustomNodes;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(NCreatureVisuals))]
internal class CounterPileCreatureVisualsPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(NCreatureVisuals._Ready))]
    public static void Postfix(NCreatureVisuals __instance)
    {
        string counterPileScenePath = "res://scenes/sts2chartest/counter_pile.tscn";
        NCounterPile nCounterPile = GD.Load<PackedScene>(counterPileScenePath).Instantiate<NCounterPile>();
        CounterPileResource.CreatureVisualsCounterPile.Set(__instance, nCounterPile);
        Player player = CounterPileResource.CreatureVisualsPlayer.Get(__instance);
        nCounterPile._pile = CardPile.Get(CounterPile.Counter, player);
        __instance.AddChild(nCounterPile);
        nCounterPile.Position = new Vector2(250, -250);
    }
}