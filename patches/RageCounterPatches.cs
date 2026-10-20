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
internal class RageCounterPatch1
{

    [HarmonyPrefix]
    public static void CreateRageCounter(ref NCombatUi __instance)
    {
        NRageCounter rageCounter = ResourceLoader.Load<PackedScene>("res://scenes/sts2chartest/rage_counter.tscn")
            .Instantiate<NRageCounter>();
        __instance.AddChildSafely(rageCounter);
        RageResource._rageCounter.Set(__instance, rageCounter);
    }
}

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal class RageCounterPatch2
{
    [HarmonyPrefix]
    public static void GetPreAnimPosition(ref NCombatUi __instance, out Vector2 __state)
    {
        __state = __instance.EnergyCounterContainer.GlobalPosition;
    }
    
    [HarmonyPostfix]
    public static void InitializeRageCounter(ref NCombatUi __instance, ref CombatState state, ref NEnergyCounter ____energyCounter, Vector2 __state)
    {
        Player me = LocalContext.GetMe(state);
        NRageCounter rageCounter = RageResource._rageCounter.Get(__instance);
        if (rageCounter == null)
            return;
        rageCounter.Initialize(me);
        Vector2 offset = rageCounter.GlobalPosition - __state;
        rageCounter.Reparent(____energyCounter, false);
        rageCounter.Position = offset;
    }
}