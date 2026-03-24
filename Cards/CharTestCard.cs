using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using StS2CharTest.Powers;

namespace StS2CharTest.Cards;

public abstract class CharTestCard(int cost, CardType cardType, CardRarity cardRarity, TargetType targetType) : CustomCardModel(cost,
    cardType, cardRarity,
    targetType)
{
    public override string CustomPortraitPath => "res://images/packed/card_portraits/" + Id.Entry.RemovePrefix().ToLowerInvariant() + ".png";
    public override string PortraitPath => "res://images/packed/card_portraits/big/" + Id.Entry.RemovePrefix().ToLowerInvariant() + ".png";

    public async Task TriggerBlaze()
    {
        foreach (Creature hittableCreature in CombatState.HittableEnemies)
        {
            CharTestPowerModel charTestPower = null;
            foreach (PowerModel power in hittableCreature.Powers)
            {
                if(power.GetType() == typeof(CharTestPowerModel))
                    charTestPower = (power as CharTestPowerModel);
            }
            if (charTestPower != null)
                await charTestPower.OnBlazeTriggered();
        }
    }
}