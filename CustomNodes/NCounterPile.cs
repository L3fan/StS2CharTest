using Godot;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using StS2CharTest.CustomNodes;

public partial class NCounterPile : NCombatCardPile
{
	public string cardNodePath = "res://scenes/cards/card.tscn";
	
	public void Initialize(CardPile pile)
	{
		_pile = pile;
		_pile.CardAdded += CardAdded;
		
	}

	protected override PileType Pile => CounterPile.Counter;

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
	public static readonly SpireField<NCreatureVisuals, NCounterPile> CreatureVisualsCounterPile = new(() => null);
	public static readonly SpireField<PlayerCombatState, CardPile> PlayerCombatStateCounterPile = new(() => null);
	public static readonly SpireField<NCreatureVisuals, Player> CreatureVisualsPlayer = new(() => null);
}
