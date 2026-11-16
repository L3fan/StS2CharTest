using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using StS2CharTest.Code.Character;
using StS2CharTest.Powers;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(NHealthBar))]
internal class CustomHpBarPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(NHealthBar._Ready))]
    public static void Postfix(NHealthBar __instance, ref Creature ____creature, ref Control ____poisonForeground)
    {
        Control _rageForeground = ____poisonForeground.Duplicate() as Control;
        _rageForeground.Name = "RageForeground";
        _rageForeground.SelfModulate = new Color(1.0f, 0.7f, 0.1f);
        //MainFile.Logger.Info("Adding: " + _rageForeground.GetType() + " " + _rageForeground.Name);
        ____poisonForeground.GetParent().AddChildSafely(_rageForeground);
        ____poisonForeground.GetParent().MoveChild(_rageForeground, ____poisonForeground.GetIndex()+1);
    }

    [HarmonyPostfix, HarmonyPatch("RefreshForeground")]
    public static void Postfix(NHealthBar __instance, ref Creature ____creature, ref float ____expectedMaxFgWidth, ref Control ____hpForegroundContainer, ref Control ____hpForeground)
    {
        Control _rageForeground = __instance.GetNode<Control>("%Mask/RageForeground");
        if (!____creature.IsPlayer)
        {
            _rageForeground.Visible = ____creature.IsPlayer;
            return;
        }

        Player player = ____creature.Player;
        
        if (____creature.CurrentHp <= 0)
        {
            _rageForeground.Visible = false;
        } else if (!____creature.HpDisplay.IsInfinite())
        {
            int powerAmount = ____creature.GetPowerAmount<DoomPower>();
            PoisonPower poisonPower = ____creature.GetPower<PoisonPower>();
            int rageAmount = (int)RageResource.Amount.Get(____creature.CombatState);
            int poisonDamageNextTurn = poisonPower != null ? poisonPower.CalculateTotalDamageNextTurn() : 0;
            float num1 = GetFgWidth(____creature, ____creature.CurrentHp - poisonDamageNextTurn, GetMaxFgWidth(____hpForegroundContainer, ____expectedMaxFgWidth)) - GetMaxFgWidth(____hpForegroundContainer, ____expectedMaxFgWidth);
            if (rageAmount > 0)
            {
                _rageForeground.Visible = true;
                //if poison isn't lethal
                if (____creature.CurrentHp + rageAmount > poisonDamageNextTurn)
                {
                    float fgWidth = GetFgWidth(____creature, ____creature.CurrentHp - poisonDamageNextTurn,
                        GetMaxFgWidth(____hpForegroundContainer, ____expectedMaxFgWidth));
                    ____hpForeground.OffsetRight =
                        fgWidth - GetMaxFgWidth(____hpForegroundContainer, ____expectedMaxFgWidth);
                    //MainFile.Logger.Info("HP Foreground Offset: " + _rageForeground.OffsetRight);
                    ____hpForeground.Visible = true;
                    int patchMarginLeft = ((NinePatchRect)_rageForeground).PatchMarginLeft;
                    _rageForeground.OffsetLeft = Math.Max(0.0f, fgWidth - patchMarginLeft);
                    float fgWidthSansPoison = GetFgWidth(____creature, ____creature.CurrentHp + rageAmount - poisonDamageNextTurn,
                        GetMaxFgWidth(____hpForegroundContainer, ____expectedMaxFgWidth));
                    _rageForeground.OffsetRight = fgWidthSansPoison -
                                                  GetMaxFgWidth(____hpForegroundContainer, ____expectedMaxFgWidth);
                    //MainFile.Logger.Info("Rage Bar Offset: " + _rageForeground.OffsetRight);
                }
                else
                {
                    _rageForeground.Visible = false;
                }
            }
            else
            {
                _rageForeground.Visible = false;
                _rageForeground.OffsetLeft = 0.0f;
            }
        }
    }

    public static float GetFgWidth(Creature creature, float amount, float maxFgWidth)
    {
        int rageAmount = 0;
        if(creature.IsPlayer)
            rageAmount = (int)RageResource.Amount.Get(creature.CombatState);
        return creature.MaxHp <= 0 ? 0.0f : float.Max((float)amount / ((float)creature.MaxHp + rageAmount) * maxFgWidth, creature.CurrentHp > 0 ? 12f : 0.0f);
    }

    public static float GetMaxFgWidth(Control hpForegroundContainer, float expectedMaxFgWidth)
    {
        return (double) expectedMaxFgWidth <= 0.0 ? hpForegroundContainer.Size.X : expectedMaxFgWidth;
    }

    [HarmonyPatch("RefreshText")]
    public static void Postfix(ref MegaLabel ____hpLabel, ref Creature ____creature)
    {
        if (____creature.CurrentHp <= 0)
            return;
        
        if (!____creature.IsPlayer)
            return;
        
        Player player = ____creature.Player;
        int rageAmount = 0;
        if(____creature.IsPlayer)
            rageAmount = (int)RageResource.Amount.Get(____creature.CombatState);
        ____hpLabel.SetTextAutoSize($"{____creature.CurrentHp + rageAmount}/{____creature.MaxHp}");
    }
}