using Godot;
using System;
using BaseLib.Patches.Content;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;

public partial class NCounterPile : NCombatCardPile
{

	public string cardNodePath = "res://scenes/cards/card.tscn";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	protected override PileType Pile { get; }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

public class CounterPileResource
{
	[CustomEnum] 
	public static PileType Counter;
	public static readonly SpireField<NCreatureVisuals, NCounterPile> CreatureVisualsCounterPile = new(() => null);
	public static readonly SpireField<PlayerCombatState, CardPile> PlayerCombatStateCounterPile = new(() => null);
}
