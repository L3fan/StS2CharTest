using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(Burn), "OnTurnEndInHand")]
internal class BurnEtherealPatch
{
    private static void Postfix(Burn __instance, PlayerChoiceContext choiceContext)
    {
        if(__instance.Keywords.Contains(CardKeyword.Exhaust))
            CardCmd.Exhaust(choiceContext, __instance);
    }
}