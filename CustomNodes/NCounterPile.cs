using Godot;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using StS2CharTest.CustomNodes;

public partial class NCounterPile : NButton
{
	public string cardNodePath = "res://scenes/cards/card.tscn";

	private CardPile? _pile;
	
	public void Initialize(Player player)
	{
		_pile = CardPile.Get(CounterPile.Counter, player);
		if(_pile != null)
			_pile.CardAdded += CardAdded;
		
	}

	protected PileType Pile => CounterPile.Counter;

	public void CardAdded(CardModel card)
	{
		if (_pile.Cards.Count > 3)
			return;

		NCard newCard = GD.Load<PackedScene>(cardNodePath).Instantiate<NCard>();
		newCard.Model = card;
		newCard.UpdateVisuals(Pile, CardPreviewMode.Normal);
		AddChild(newCard);
		newCard.Position = GetTargetPosition(_pile.Cards.IndexOf(card));

	}

	private Vector2 GetTargetPosition(int index)
	{
		int offset = (index - Mathf.FloorToInt(Mathf.Min(_pile.Cards.Count, 3)/2f)) * -15;

		return Vector2.Right * offset;
	}
}

public class CounterPileResource
{
	public static readonly SpireField<NCreature, CounterPile> NCreatureCounterPile = new(() => null);
}
