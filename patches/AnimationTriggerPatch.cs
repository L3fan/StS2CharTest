using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using StS2CharTest.CustomNodes;

namespace StS2CharTest.patches;


[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]


internal class AnimationTriggerPatch
{
    [HarmonyPrefix]
    private static void TriggerGodotAnimation(NCreature __instance, ref string trigger)
    {
        /*MainFile.Logger.Info("Animation triggered on: " + __instance.Entity.Name);
        if (__instance.Entity.IsPlayer)
        {
            MainFile.Logger.Info("Player: " + __instance.Entity.Player.Character.Title);
            MainFile.Logger.Info("Is Visuals of NCreature Custom: " +
                                 (__instance.Visuals.GetType() == typeof(CustomCreatureVisuals)));
            if (__instance.Visuals.GetType() == typeof(CustomCreatureVisuals))
            {
                CustomCreatureVisuals customVisuals = (CustomCreatureVisuals)__instance.Visuals;
                customVisuals.OnAnimationTriggered(trigger);
            }
        }*/
        
    }
    
}