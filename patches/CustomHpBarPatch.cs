using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(NHealthBar))]
internal class CustomHpBarPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(NHealthBar._Ready))]
    public static void Postfix(NHealthBar __instance, ref Control ____poisonForeground)
    {
        Control _embersForeground = ____poisonForeground.Duplicate() as Control;
        _embersForeground.Name = "EmbersForeground";
        _embersForeground.SelfModulate = new Color(1.0f, 0.7f, 0.1f);
        _embersForeground.Visible = true;
        MainFile.Logger.Info("Adding: " + _embersForeground.GetType() + " " + _embersForeground.Name);
        __instance.GetNode<Control>("%Mask").AddChildSafely(_embersForeground);
        __instance.GetNode<Control>("%Mask").MoveChild(_embersForeground, 2);
    }

    [HarmonyPostfix, HarmonyPatch("RefreshForeground")]
    public static void Postfix(NHealthBar __instance, ref Creature ____creature)
    {
        Control _embersForeground = __instance.GetNode<Control>("%Mask/EmbersForeground");
        if (____creature.CurrentHp <= 0)
        {
            _embersForeground.Visible = false;
        } else if (!____creature.ShowsInfiniteHp)
        {
            
        }
    }
}