using BaseLib.Patches.Content;
using Godot;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using StS2CharTest;
using StS2CharTest.CustomNodes;

public partial class NCounterPile : NCombatCardPile
{
	public string cardNodePath = "res://scenes/cards/card.tscn";

	private CardPile? _pile;

	public override void _Ready()
	{
		this.ConnectSignals();
		this._emptyPileMessage = new LocString("combat_messages", "OPEN_EMPTY_COUNTER");
	}
	
	public void Initialize(Player? player)
	{
		MainFile.Logger.Info("PlayerCombatState is null? " + (player.PlayerCombatState == null));
		MainFile.Logger.Info("Counter enum: " + CounterPile.Counter);
		_pile = CustomPiles.GetCustomPile(player.PlayerCombatState, CounterPile.Counter);
		MainFile.Logger.Info("Gotten Counter Pile is null? " + (_pile == null));
		if (_pile != null)
		{
			_pile.CardAdded += CardAdded;
			_pile.CardAddFinished += CardAddFinished;
			_pile.ContentsChanged += ContentsChanged;
		}

	}

	protected override PileType Pile => CounterPile.Counter;

	public void CardAdded(CardModel card)
	{
		MainFile.Logger.Info("Added card '" + card.Title + "' to Counter Pile");
		if (_pile.Cards.Count > 3)
			return;

		NCard newCard = GD.Load<PackedScene>(cardNodePath).Instantiate<NCard>();
		newCard.Model = card;
		newCard.UpdateVisuals(Pile, CardPreviewMode.Normal);
		AddChild(newCard);
		newCard.Position = GetTargetPosition(_pile.Cards.IndexOf(card));

	}

	public void CardAddFinished()
	{
		MainFile.Logger.Info("Card got added to Counter Pile");
	}
	
	
	public void ContentsChanged()
	{
		MainFile.Logger.Info("NCounterPile: Contents of Counter Pile changed");
	}

	private Vector2 GetTargetPosition(int index)
	{
		int offset = (index - Mathf.FloorToInt(Mathf.Min(_pile.Cards.Count, 3)/2f)) * -15;

		return Vector2.Right * offset;
	}
}

public class CounterPileResource
{
	public static readonly SpireField<NCreatureVisuals, NCounterPile> NCreatureVisualsNCounterPile = new(() => null);
}
