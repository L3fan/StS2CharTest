using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal class NCombatUIActivatePatch
{
    [HarmonyPostfix]
    public static void ActivatePostfix(NCombatUi __instance, CombatState state)
    {
        MainFile.Logger.Info("NCombatUi Activate Patch Test!");
        
        var container = __instance.GetNode<NCombatPilesContainer>("%CombatPileContainer");
        /*string pileNames = "";
        foreach (Node child in container.GetChildren())
        {
            pileNames += child.Name + ", ";
        }
        MainFile.Logger.Info("Piles: " + pileNames);*/
        var counterPile = container.GetNodeOrNull<NCounterPile>("CounterPile");
        var player = LocalContext.GetMe(state);
        if (player == null) return;

        counterPile?.Initialize(player);
    }
}