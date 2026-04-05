using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using StS2CharTest.Cards;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(CardModel))]
internal class CardModelHeatPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(CardModel.SetToFreeThisTurn))]
    public static void FreeHeatThisTurn(ref CardModel __instance)
    {
        if(__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
            ((CharTestCard)__instance).SetHeatCostThisTurn(0);
    }
    
    [HarmonyPostfix, HarmonyPatch(nameof(CardModel.SetToFreeThisCombat))]
    public static void FreeHeatThisCombat(ref CardModel __instance)
    {
        if(__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
            ((CharTestCard)__instance).SetHeatCostThisCombat(0);
    }

    [HarmonyPrefix, HarmonyPatch(nameof(CardModel.EndOfTurnCleanup))]
    public static void EndOfTurnCleanupPatch(ref CardModel __instance)
    {
        if (__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
        { 
            CharTestCard charTestCard =  (CharTestCard)__instance;
            charTestCard.EndOfTurnCleanupHeat();
        }
    }

    [HarmonyPrefix, HarmonyPatch(nameof(CardModel.SpendResources))]
    public static void SpendResourcesHeatPatch(ref CardModel __instance)
    {
        if (__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
        {
            CharTestCard charTestCard = (CharTestCard)__instance;
            int heatToSpend = Math.Max(0, charTestCard.GetHeatCostWithModifiers());
            charTestCard.SpendHeat(heatToSpend);
        }
    }

    [HarmonyPostfix, HarmonyPatch(nameof(CardModel.OnPlayWrapper))]
    public static void OnPlayWrapperHeatPatch(ref CardModel __instance)
    {
        if (__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
        {
            CharTestCard charTestCard = (CharTestCard)__instance;
            charTestCard.ResetTempHeatCost();
        }
    }


    [HarmonyPostfix, HarmonyPatch(nameof(CardModel.FinalizeUpgradeInternal))]
    public static void FinalizeUpgradeInternalHeatPatch(ref CardModel __instance)
    {
        if (__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
        {
            CharTestCard charTestCard = (CharTestCard)__instance;
            charTestCard.SetHeatCostWasJustUpgraded(false);
        }
    }

    [HarmonyPrefix, HarmonyPatch(nameof(CardModel.DowngradeInternal))]
    public static void DowngradeInternalHeatPatch(ref CardModel __instance)
    {
        if (__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
        {
            CardModel mutable = ModelDb.GetById<CardModel>(__instance.Id).ToMutable();
            CharTestCard charTestCard = (CharTestCard)mutable;
            charTestCard.DowngradeHeatCost(charTestCard);
        }
    }
}