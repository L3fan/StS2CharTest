using BaseLib.Patches.Content;
using Godot;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using StS2CharTest;
using StS2CharTest.CustomNodes;

public partial class NCounterPile : NCombatCardPile
{
	public string cardNodePath = "res://scenes/cards/card.tscn";

	private CardPile? _pile;

	public List<NCard> VisibleCards;

	public Control cardHoldersContainer;

	public override void _Ready()
	{
		this.ConnectSignals();
		this._emptyPileMessage = new LocString("combat_messages", "OPEN_EMPTY_COUNTER");
		cardHoldersContainer = GetNode<Control>("%CardHolderContainer");
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
			_pile.CardRemoved += CardRemoved;
			_pile.ContentsChanged += ContentsChanged;
		}

	}

	protected override PileType Pile => CounterPile.Counter;

	public void CardAdded(CardModel card)
	{
		MainFile.Logger.Info("Added card '" + card.Title + "' to Counter Pile");
		if (_pile.Cards.Count > 3)
			return;

		NCard cardNode = NCard.Create(card);
		NPreviewCardHolder holder = NPreviewCardHolder.Create(cardNode, true, true);
		holder.SetCardScale(Vector2.One * 0.5f);
		cardNode.UpdateVisuals(Pile, CardPreviewMode.Normal);
		OnCardAdded(holder);

	}

	public void OnCardAdded(NPreviewCardHolder newHolder)
	{
		cardHoldersContainer.AddChildSafely(newHolder);
		cardHoldersContainer.MoveChild(newHolder, 0);
		
		UpdateCardHoldersContainer();
	}

	public void UpdateCardHoldersContainer()
	{
		int numOfShownCards = Mathf.Min(_pile.Cards.Count, 3);
		if (numOfShownCards > 0)
		{
			for (int i = 0; i < cardHoldersContainer.GetChildren().Count; i++)
			{
				Node child = cardHoldersContainer.GetChild(i);
				if (child is not NPreviewCardHolder)
					continue;
				NPreviewCardHolder holder = (NPreviewCardHolder)child;
				NCard card = holder.CardNode;
				Control cardFrame = card.Body.GetNode<Control>("%Frame");
				holder.Position = new Vector2(i * cardFrame.Size.X / 2f * holder.Scale.X, 0);
				MainFile.Logger.Info("Child " + i + " Position: " + holder.Position.X);
				
				card.UpdateVisuals(Pile, CardPreviewMode.Normal);
			}
		}
	}

	public void CardRemoved(CardModel removedCard)
	{
		MainFile.Logger.Info("Card got removed from Counter Pile");
		Node child = cardHoldersContainer.GetChild(cardHoldersContainer.GetChildCount() - 1);
		if (child is NPreviewCardHolder)
		{
			NPreviewCardHolder holder = (NPreviewCardHolder)child;
			holder.QueueFree();
		}

		if (_pile.Cards.Count >= 3)
		{
			NCard cardNode = NCard.Create(_pile.Cards[2]);
			NPreviewCardHolder holder = NPreviewCardHolder.Create(cardNode, true, true);
			holder.SetCardScale(Vector2.One * 0.5f);
			cardNode.UpdateVisuals(Pile, CardPreviewMode.Normal);
			OnCardAdded(holder);
		}
		
		UpdateCardHoldersContainer();
	}
	
	
	public void ContentsChanged()
	{
		MainFile.Logger.Info("NCounterPile: Contents of Counter Pile changed");
		/*if (_pile.IsEmpty)
			return;

		VisibleCards.Clear();
		for (int i = 0; i < _pile.Cards.Count; i++)
		{
			CardModel card = _pile.Cards[i];
			NCard newCard = GD.Load<PackedScene>(cardNodePath).Instantiate<NCard>();
			newCard.Model = card;
			newCard.UpdateVisuals(Pile, CardPreviewMode.Normal);
			AddChild(newCard);
			//newCard.Position = GetTargetPosition(_pile.Cards.IndexOf(card))
			newCard.Position = new Vector2((_pile.Cards.Count - i - 1) / (float)_pile.Cards.Count, 0);
			VisibleCards.Add(newCard);
		}
		
		MainFile.Logger.Info("Counterpile cards: " + _pile.Cards.Count);*/
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
