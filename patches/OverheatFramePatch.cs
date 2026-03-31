using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using StS2CharTest.Cards;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(CardModel), "FrameMaterial", MethodType.Getter)]
internal class OverheatFramePatch
{
    [HarmonyPrefix]
    public static bool UseOverheatColor(CardModel __instance, ref Material __result)
    {
        if(!__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
            return true;
        CharTestCard c = (CharTestCard) __instance;
        __result = ShaderUtils.GenerateHsv(c.CustomBackgroundColor.H, c.CustomBackgroundColor.S,
            c.CustomBackgroundColor.V);
        return false;
    }
}