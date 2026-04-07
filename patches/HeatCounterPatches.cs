using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using StS2CharTest.Cards;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi._Ready))]
internal class HeatCounterPatch1
{

    [HarmonyPrefix]
    public static void CreateHeatCounter(ref NCombatUi __instance)
    {
        NHeatCounter heatCounter = ResourceLoader.Load<PackedScene>("res://scenes/sts2chartest/heat_counter.tscn")
            .Instantiate<NHeatCounter>();
        __instance.AddChildSafely(heatCounter);
        HeatResource._heatCounter.Set(__instance, heatCounter);
    }
}

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal class HeatCounterPatch2
{
    [HarmonyPrefix]
    public static void GetPreAnimPosition(ref NCombatUi __instance, out Vector2 __state)
    {
        __state = __instance.EnergyCounterContainer.GlobalPosition;
    }
    
    [HarmonyPostfix]
    public static void InitializeHeatCounter(ref NCombatUi __instance, ref CombatState state, ref NEnergyCounter ____energyCounter, Vector2 __state)
    {
        Player me = LocalContext.GetMe(state);
        NHeatCounter heatCounter = HeatResource._heatCounter.Get(__instance);
        if (heatCounter == null)
            return;
        heatCounter.Initialize(me);
        Vector2 offset = heatCounter.GlobalPosition - __state;
        heatCounter.Reparent(____energyCounter, false);
        heatCounter.Position = offset;
    }
}