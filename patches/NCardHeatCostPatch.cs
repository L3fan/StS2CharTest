using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using StS2CharTest.Cards;
using StS2CharTest.CustomNodes;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(NCard), nameof(NCard._Ready))]
internal class NCardHeatCostPatch
{
    [HarmonyPostfix]
    public static void AddHeatCostIcon(ref NCard __instance)
    {
        TextureRect heatIcon = __instance.GetNode<TextureRect>((NodePath) "%StarIcon").Duplicate() as TextureRect;
        heatIcon.Name = "HeatIcon";
        heatIcon.Texture = PreloadManager.Cache.GetTexture2D("res://images/sts2chartest/ui/heat_counter_bg.png");
        HeatResource.heatCostIcon.Set(__instance, heatIcon);
        
        MegaLabel heatLabel = heatIcon.GetChild<MegaLabel>(0);
        heatLabel.Name = "HeatLabel";
        HeatResource.heatCostLabel.Set(__instance, heatLabel);
        
        __instance.GetNode((NodePath)"%CardContainer").AddChildSafely(heatIcon);
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
internal class NCardHeatCostPatch2
{
    [HarmonyPrefix]
    public static void UpdateHeatCostIcon(ref NCard __instance, PileType pileType, ref bool ____pretendCardCanBePlayed)
    {
        if (!__instance.IsNodeReady())
            return;
        if (__instance.Model == null)
            throw new InvalidOperationException("Cannot update text with no model.");
        NCardHeatCostVisuals.UpdateHeatCostVisuals(__instance, pileType, ____pretendCardCanBePlayed);
    }
}