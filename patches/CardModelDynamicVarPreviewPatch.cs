using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using StS2CharTest.CustomNodes;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.UpdateDynamicVarPreview))]
internal class CardModelDynamicVarPreviewPatch
{
    [HarmonyPostfix]
    static void Postfix(CardModel __instance, CardPreviewMode previewMode, Creature? target, DynamicVarSet dynamicVarSet)
    {
        //if(__instance.Pile != null)
        if(__instance.Pile?.Type == CounterPile.Counter)
            foreach (DynamicVar dynamicVar in (IEnumerable<DynamicVar>) dynamicVarSet.Values.ToList<DynamicVar>())
                dynamicVar.UpdateCardPreview(__instance, previewMode, target, true);
    }
}