using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace StS2CharTest.CustomNodes;

public partial class CustomCardHolder : NCardHolder
{
    public override Vector2 SmallScale => Vector2.One * 0.5f;
}