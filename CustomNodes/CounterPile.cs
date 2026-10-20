using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using BaseLib.Patches.Content;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace StS2CharTest.CustomNodes;

public class CounterPile : CustomPile
{
    [CustomEnum] 
    public static PileType Counter;

    [CustomEnum] 
    public static AutoPlayType CounterPlay;

    public event Action CounterCardPlayed;

    public Vector2 localTargetPosition = new Vector2(750, 400);

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
        NCreature? creature = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        if(creature == null)
            return Vector2.Zero;
        return localTargetPosition;
    }
    
    public void AddInternal(CardModel card, int index = -1, bool silent = false)
    {
        MainFile.Logger.Info("Adding Counter Pile");
        base.AddInternal(card);
    }
    

    public async Task PlayerWasAttacked(Creature damageSource)
    {
        if (damageSource == null)
            return;
		await TriggerMostRecentCardInPile(damageSource);
    }

    public async Task TriggerMostRecentCardInPile(Creature source)
    {
        if (Cards == null)
            return;
        if (Cards.Count <= 0)
            return;
        CardModel cardToPlay = Cards.First();
        InvokeCounterCardPlayed();
        await CardCmd.AutoPlay(new BlockingPlayerChoiceContext(), cardToPlay, source, CounterPlay);
    }

    public void InvokeCounterCardPlayed()
    {
        CounterCardPlayed?.Invoke();
    }
}