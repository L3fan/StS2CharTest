using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
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
    [CustomEnum] 
    public static PileType Counter;
    public CounterPile() : base(Counter)
    {
        ContentsChanged += OnContentsChanged;
    }

    public override bool CardShouldBeVisible(CardModel card)
    {
        return false;
    }

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        Player player = model.Owner;
        NCreature? creature = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        if(creature == null)
            return Vector2.Zero;
        return creature.Visuals.GlobalPosition + new Vector2(160, -175);
    }
    
    public void AddInternal(CardModel card, int index = -1, bool silent = false)
    {
        MainFile.Logger.Info("Adding Counter Pile");
        base.AddInternal(card);
    }
    
	
    public void OnContentsChanged()
    {
        MainFile.Logger.Info("Counter Pile: Contents of Counter Pile changed");
    }
}