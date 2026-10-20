using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using StS2CharTest.Cards;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(CardModel))]
internal class CardModelRagePatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(CardModel.SetToFreeThisTurn))]
    public static void FreeRageThisTurn(ref CardModel __instance)
    {
        if(__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
            ((CharTestCard)__instance).SetRageCostThisTurn(0);
    }
    
    [HarmonyPostfix, HarmonyPatch(nameof(CardModel.SetToFreeThisCombat))]
    public static void FreeRageThisCombat(ref CardModel __instance)
    {
        if(__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
            ((CharTestCard)__instance).SetRageCostThisCombat(0);
    }

    [HarmonyPrefix, HarmonyPatch(nameof(CardModel.EndOfTurnCleanup))]
    public static void EndOfTurnCleanupPatch(ref CardModel __instance)
    {
        if (__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
        { 
            CharTestCard charTestCard =  (CharTestCard)__instance;
            charTestCard.EndOfTurnCleanupRage();
        }
    }

    [HarmonyPrefix, HarmonyPatch(nameof(CardModel.SpendResources))]
    public static void SpendResourcesRagePatch(ref CardModel __instance)
    {
        if (__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
        {
            CharTestCard charTestCard = (CharTestCard)__instance;
            int rageToSpend = Math.Max(0, charTestCard.GetRageCostWithModifiers());
            charTestCard.SpendRage(rageToSpend);
        }
    }

    [HarmonyPostfix, HarmonyPatch(nameof(CardModel.OnPlayWrapper))]
    public static void OnPlayWrapperRagePatch(ref CardModel __instance)
    {
        if (__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
        {
            CharTestCard charTestCard = (CharTestCard)__instance;
            charTestCard.ResetTempRageCost();
        }
    }


    [HarmonyPostfix, HarmonyPatch(nameof(CardModel.FinalizeUpgradeInternal))]
    public static void FinalizeUpgradeInternalRagePatch(ref CardModel __instance)
    {
        if (__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
        {
            CharTestCard charTestCard = (CharTestCard)__instance;
            charTestCard.SetRageCostWasJustUpgraded(false);
        }
    }

    [HarmonyPrefix, HarmonyPatch(nameof(CardModel.DowngradeInternal))]
    public static void DowngradeInternalRagePatch(ref CardModel __instance)
    {
        if (__instance.GetType().IsSubclassOf(typeof(CharTestCard)))
        {
            CardModel mutable = ModelDb.GetById<CardModel>(__instance.Id).ToMutable();
            CharTestCard charTestCard = (CharTestCard)mutable;
            charTestCard.DowngradeRageCost(charTestCard);
        }
    }
}