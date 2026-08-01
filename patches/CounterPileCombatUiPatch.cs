using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using StS2CharTest.CustomNodes;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(NCombatUi))]
internal class CounterPileCombatUiPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(NCombatUi.Activate))]
    public static void Postfix(NCombatUi __instance, ref PlayerCombatState ____state)
    {
        Player me = LocalContext.GetMe((ICombatState) ____state);
        CounterPileResource.CreatureVisualsPlayer.Set(me.Creature.GetCreatureNode().Visuals, me);
    }
}