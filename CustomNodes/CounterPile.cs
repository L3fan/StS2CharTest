using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace StS2CharTest.CustomNodes;

public class CounterPile : CustomPile
{
    public CounterPile(PileType pileType) : base(pileType)
    {
    }

    public override bool CardShouldBeVisible(CardModel card)
    {
        return Cards.IndexOf(card) < 3;

    }

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        if (Cards.IndexOf(model) < 3)
            return Vector2.Zero;
        size = Vector2.One * 0.5f;

        Vector2 position = Vector2.Right * ((Cards.IndexOf(model) - 1) * -15);
        return position;
    }
}