using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using StS2CharTest.Powers;

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
    public static void Postfix(NHealthBar __instance, ref Creature ____creature, ref float ____expectedMaxFgWidth, ref Control ____hpForegroundContainer, ref Control ____hpForeground)
    {
        Control _embersForeground = __instance.GetNode<Control>("%Mask/EmbersForeground");
        if (____creature.CurrentHp <= 0)
        {
            _embersForeground.Visible = false;
        } else if (!____creature.ShowsInfiniteHp)
        {
            int powerAmount = ____creature.GetPowerAmount<DoomPower>();
            PoisonPower poisonPower = ____creature.GetPower<PoisonPower>();
            EmbersPower emberPower = ____creature.GetPower<EmbersPower>();
            int poisonDamageNextTurn = poisonPower != null ? poisonPower.CalculateTotalDamageNextTurn() : 0;
            int embersDamageNextTurn = emberPower != null ? emberPower.CalculateTotalDamage() : 0;
            int totalDamageNextTurn = poisonDamageNextTurn + embersDamageNextTurn;

            float num1 = GetFgWidth(____creature, ____creature.CurrentHp, GetMaxFgWidth(____hpForegroundContainer, ____expectedMaxFgWidth)) - GetMaxFgWidth(____hpForegroundContainer, ____expectedMaxFgWidth);
            if (____creature.HasPower<EmbersPower>())
            {
                _embersForeground.Visible = true;
                //if poison isn't lethal
                if (____creature.CurrentHp > poisonDamageNextTurn)
                {
                    //if embers is lethal
                    if (____creature.CurrentHp <= embersDamageNextTurn)
                    {
                        _embersForeground.OffsetLeft = 0.0f;
                        _embersForeground.OffsetRight = num1;
                        ____hpForeground.Visible = false;
                        MainFile.Logger.Info("Embers Offset: " + _embersForeground.OffsetRight);
                    }
                    else
                    {
                        float fgWidth = GetFgWidth(____creature, ____creature.CurrentHp - totalDamageNextTurn, GetMaxFgWidth(____hpForegroundContainer, ____expectedMaxFgWidth));
                        ____hpForeground.OffsetRight = fgWidth - GetMaxFgWidth(____hpForegroundContainer, ____expectedMaxFgWidth);
                        //MainFile.Logger.Info("HP Foreground Offset: " + _embersForeground.OffsetRight);
                        ____hpForeground.Visible = true;
                        int patchMarginLeft = ((NinePatchRect) _embersForeground).PatchMarginLeft;
                        _embersForeground.OffsetLeft = Math.Max(0.0f, fgWidth - patchMarginLeft);
                        float fgWidthSansPoison = GetFgWidth(____creature, ____creature.CurrentHp - poisonDamageNextTurn, GetMaxFgWidth(____hpForegroundContainer, ____expectedMaxFgWidth));
                        _embersForeground.OffsetRight = fgWidthSansPoison - GetMaxFgWidth(____hpForegroundContainer, ____expectedMaxFgWidth);
                        //MainFile.Logger.Info("Embers Offset: " + _embersForeground.OffsetRight);
                    }
                }
                else
                {
                    _embersForeground.Visible = false;
                }
            }
            else
            {
                _embersForeground.Visible = false;
                _embersForeground.OffsetLeft = 0.0f;
            }
        }
    }

    public static float GetFgWidth(Creature creature, float amount, float maxFgWidth)
    {
        return creature.MaxHp <= 0 ? 0.0f : float.Max((float)amount / (float)creature.MaxHp * maxFgWidth, creature.CurrentHp > 0 ? 12f : 0.0f);
    }

    public static float GetMaxFgWidth(Control hpForegroundContainer, float expectedMaxFgWidth)
    {
        return (double) expectedMaxFgWidth <= 0.0 ? hpForegroundContainer.Size.X : expectedMaxFgWidth;
    }
}