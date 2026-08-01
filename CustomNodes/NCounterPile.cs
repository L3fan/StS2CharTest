using Godot;
using System;
using MegaCrit.Sts2.Core.Nodes.Cards;

public partial class NCounterPile : Node2D
{
	public SubViewport subViewport;

	public string cardNodePath = "res://scenes/cards/card.tscn";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		subViewport = GetNode<SubViewport>("%SubViewport");
		NCard cardFront = GetNode<NCard>(cardNodePath);
		cardFront.Position += Vector2.Right * 10;
		NCard cardMiddle = GetNode<NCard>(cardNodePath);
		NCard cardBack = GetNode<NCard>(cardNodePath);
		cardBack.Position += Vector2.Left * 10;
		subViewport?.AddChild(cardBack);
		subViewport?.AddChild(cardMiddle);
		subViewport?.AddChild(cardFront);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
