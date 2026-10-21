using BaseLib.Patches.Content;
using Godot;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using StS2CharTest;
using StS2CharTest.CustomNodes;

public partial class NCounterPile : NCombatCardPile
{
	public string cardNodePath = "res://scenes/cards/card.tscn";

	private CardPile? _pile;

	public List<NCard> VisibleCards;

	public Control cardHoldersContainer;

	public CardModel cardToAdd;

	private Tween _cardPlayTween;
	private Vector2 _cardPlayOutPosition = new Vector2(128, 0);
	private Color _cardPlayOutColor = new Color(1, 1, 1, 0);
	private Vector2 _cardPlayInPosition = new Vector2(0, -128);
	private Color _cardPlayInColor = new Color(1, 1, 1, 1);

	private List<Tween> _cardMoveTweens;

	public override void _Ready()
	{
		this._emptyPileMessage = new LocString("combat_messages", "OPEN_EMPTY_COUNTER");
		cardHoldersContainer = GetNode<Control>("%CardHolderContainer");
		this.ConnectSignals();
		MouseEntered += MouseEnteredTest;
	}

	public void MouseEnteredTest()
	{
		MainFile.Logger.Info("Mouse Entered Counter Pile! (Test)");
	}
	
	public void Initialize(Player? player)
	{
		//MainFile.Logger.Info("PlayerCombatState is null? " + (player.PlayerCombatState == null));
		//MainFile.Logger.Info("Counter enum: " + CounterPile.Counter);
		_pile = CustomPiles.GetCustomPile(player.PlayerCombatState, CounterPile.Counter);
		//MainFile.Logger.Info("Gotten Counter Pile is null? " + (_pile == null));
		if (_pile != null)
		{
			_pile.CardAdded += CardAdded;
			_pile.CardAddFinished += CardAddFinished;
			_pile.CardRemoved += CardRemoved;
			((CounterPile)_pile).CounterCardPlayed += OnCounterCardPlayed;
		}

	}

	protected override PileType Pile => CounterPile.Counter;

	public void CardAdded(CardModel card)
	{
		MainFile.Logger.Info("Added card '" + card.Title + "' to Counter Pile");

		cardToAdd = card;
	}
	
	public void CardAddFinished()
	{
		if (_pile.Cards.Count > 3)
			return;

		if (cardToAdd != null)
		{
			NCard cardNode = NCard.Create(cardToAdd);
			AddCardHolder(cardNode);
		}

		cardToAdd = null;
	}

	public void AddCardHolder(NCard cardNode)
	{
		NPreviewCardHolder holder = NPreviewCardHolder.Create(cardNode, true, true);
		holder.SetCardScale(Vector2.One * 0.5f);
		cardNode.UpdateVisuals(Pile, CardPreviewMode.Normal);
		OnCardAdded(holder);
		PlayCardHolderAnimIn(holder);
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
				holder.Position = new Vector2(i * cardFrame.Size.X / 2f * holder.Scale.X, 0) + cardFrame.Size / 2f * holder.Scale;
				//MainFile.Logger.Info("Child " + i + " Position: " + holder.Position.X);
			}

			UpdateCardsContent();
		}
	}

	public void CardRemoved(CardModel removedCard)
	{
		MainFile.Logger.Info("Card got removed from Counter Pile");
		Node childToRemove = cardHoldersContainer.GetChild(cardHoldersContainer.GetChildCount() - 1);
		if (childToRemove is NPreviewCardHolder)
		{
			NPreviewCardHolder holder = (NPreviewCardHolder)childToRemove;
			holder.QueueFree();
		}

		//if there are more cards in the pile...
		if (_pile.Cards.Count >= 3)
		{
			//move already existing cards to the right
			if (_cardMoveTweens != null)
			{
				foreach (Tween cardMoveTween in _cardMoveTweens)
					cardMoveTween.Kill();
			}

			_cardMoveTweens = new List<Tween>();
			
			for (int i = 0; i < cardHoldersContainer.GetChildCount() - 1; i++)
			{
				NPreviewCardHolder child = cardHoldersContainer.GetChild<NPreviewCardHolder>(i);
				Control cardFrame = child.CardNode.Body.GetNode<Control>("%Frame");
				Tween newCardMoveTween = this.CreateTween();
				newCardMoveTween.TweenProperty(child, "position", new Vector2((i + 1) * cardFrame.Size.X / 2f * child.Scale.X, 0), 0.25).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);
				_cardMoveTweens.Add(newCardMoveTween);
			}
			
			//add newest card
			NCard cardNode = NCard.Create(_pile.Cards[2]);
			AddCardHolder(cardNode);;
		}
		
		UpdateCardHoldersContainer();
	}

	public void UpdateCardsContent()
	{
		foreach(NPreviewCardHolder holder in cardHoldersContainer.GetChildren())
			holder.CardNode.UpdateVisuals(Pile, CardPreviewMode.Normal);
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

	private void OnCounterCardPlayed()
	{
		Node child = cardHoldersContainer.GetChild(cardHoldersContainer.GetChildCount() - 1);
		if (child is NPreviewCardHolder)
		{
			NPreviewCardHolder holder = (NPreviewCardHolder)child;
			PlayCardHolderAnimOut(holder);
		}
	}

	private void PlayCardHolderAnimOut(NPreviewCardHolder holder)
	{
		Vector2 cardStartPosition = holder.Position;
		_cardPlayTween?.Kill();
		_cardPlayTween = this.CreateTween();
		_cardPlayTween.SetParallel(true);
		_cardPlayTween.TweenProperty((GodotObject) holder, (NodePath) "position", cardStartPosition + _cardPlayOutPosition, 0.25).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);
		_cardPlayTween.TweenProperty(holder, "modulate", _cardPlayOutColor, 0.25).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);
	}
	
	
	private void PlayCardHolderAnimIn(NPreviewCardHolder holder)
	{
		Vector2 cardEndPosition = holder.Position;
		holder.Position += _cardPlayInPosition;
		holder.Modulate = _cardPlayOutColor;
		_cardPlayTween?.Kill();
		_cardPlayTween = this.CreateTween();
		_cardPlayTween.SetParallel(true);
		_cardPlayTween.TweenProperty((GodotObject) holder, (NodePath) "position", cardEndPosition, 0.25).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);
		_cardPlayTween.TweenProperty(holder, "modulate", _cardPlayInColor, 0.25).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);
	}

	protected override void OnFocus()
	{
		if (this._pile == null)
			return;
		LocString title = new LocString("static_hover_tips", "COUNTER_PILE.title");
		LocString description = new LocString("static_hover_tips", "COUNTER_PILE.description");
		string input = ((IEnumerable<string>) this.Hotkeys).FirstOrDefault<string>();
		Key? nullable = new Key?();
		if (input != null)
			nullable = new Key?(NInputManager.Instance.GetCurrentHotkey((StringName) input));
		if (nullable.HasValue)
			title.Add("Hotkey", nullable.Value.ToString());
		else
			title.Add("Hotkey", "None");
		NHoverTipSet andShow = NHoverTipSet.CreateAndShow((Godot.Control) this, (IHoverTip) new HoverTip(title, description));
		if (andShow != null)
		{
			NHoverTipSet nhoverTipSet = andShow;
			nhoverTipSet.GlobalPosition = new Vector2(1200, 500);
		}
	}
}

public class CounterPileResource
{
	public static readonly SpireField<NCreatureVisuals, NCounterPile> NCreatureVisualsNCounterPile = new(() => null);
}
