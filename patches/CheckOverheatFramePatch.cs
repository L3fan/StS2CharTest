using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using StS2CharTest.Cards;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(NCard), "Reload")]
internal class CheckOverheatFramePatch
{
    [HarmonyPostfix]
    public static void Postfix(NCard __instance, ref TextureRect ____frame)
    {
        if (!__instance.Model.GetType().IsSubclassOf(typeof(CharTestCard)) && __instance.Visibility == ModelVisibility.Visible)
            return;
        //ShaderMaterial mat = (ShaderMaterial)____frame.Material;
        //float h = (float)mat.GetShaderParameter("h");
        MainFile.Logger.Info("Frame Texture: " + ____frame);
    }
}