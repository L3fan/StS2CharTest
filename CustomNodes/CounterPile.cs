using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace StS2CharTest.CustomNodes;

public class CounterPile : CustomPile
{
    public Action<CardModel> OnCardAdded;
    
    [CustomEnum] 
    public static PileType Counter;
    public CounterPile() : base(Counter)
    {
        
    }

    public override bool CardShouldBeVisible(CardModel card)
    {
        return false;
    }

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        Player player = model.Owner;
        NCreature creature = NCombatRoom.Instance.GetCreatureNode(player.Creature);
        return creature.Position + new Vector2(140, -175);
    }
}