using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using Microsoft.CSharp.RuntimeBinder;
using StS2CharTest.Cards;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.CanPlay), [typeof(UnplayableReason), typeof(AbstractModel)],
    [ArgumentType.Out, ArgumentType.Out])]
internal class UnplayableRageCostPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, ref bool __result, UnplayableReason reason)
    {
        ICombatState combatState = __instance.CombatState ?? __instance.Owner?.Creature.CombatState;
        if (combatState == null || __instance.Owner.PlayerCombatState == null)
            return;

        if (__instance is CharTestCard card)
        {
            if (card.CurrentRageCost > RageResource.Amount.Get(card.Owner.PlayerCombatState))
            {
                reason = NRageCounter.NotEnoughRage;
                __result = false;
            }
        }
    }
}