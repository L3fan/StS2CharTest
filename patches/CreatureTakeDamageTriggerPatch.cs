using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using StS2CharTest.CustomNodes;

namespace StS2CharTest.patches;

[HarmonyPatch(typeof(Hook), nameof(Hook.BeforeDamageReceived))]
internal class CreatureTakeDamageTriggerPatch
{
    [HarmonyPostfix]
    public static async Task Postfix(Task __result, Creature target, Creature? dealer)
    {
        await __result;
        if (!target.IsPlayer)
            return;
        Player player = target.Player;
        CounterPile counterPile = CardPile.Get(CounterPile.Counter, player) as CounterPile;
        await counterPile.PlayerWasAttacked(dealer);
    }
}