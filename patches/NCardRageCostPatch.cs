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
internal class NCardRageCostPatch
{
    [HarmonyPostfix]
    public static void AddRageCostIcon(ref NCard __instance)
    {
        TextureRect rageIcon = __instance.GetNode<TextureRect>((NodePath) "%StarIcon").Duplicate() as TextureRect;
        rageIcon.Name = "RageIcon";
        rageIcon.Texture = PreloadManager.Cache.GetTexture2D("res://images/sts2chartest/ui/rage_counter_bg.png");
        RageResource.rageCostIcon.Set(__instance, rageIcon);
        
        MegaLabel rageLabel = rageIcon.GetChild<MegaLabel>(0);
        rageLabel.Name = "RageLabel";
        RageResource.rageCostLabel.Set(__instance, rageLabel);
        
        __instance.GetNode((NodePath)"%CardContainer").AddChildSafely(rageIcon);
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
internal class NCardRageCostPatch2
{
    [HarmonyPrefix]
    public static void UpdateRageCostIcon(ref NCard __instance, PileType pileType, ref bool ____pretendCardCanBePlayed)
    {
        if (!__instance.IsNodeReady())
            return;
        if (__instance.Model == null)
            throw new InvalidOperationException("Cannot update text with no model.");
        NCardRageCostVisuals.UpdateRageCostVisuals(__instance, pileType, ____pretendCardCanBePlayed);
    }
}