using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace StS2CharTest.Cards;

public abstract class OverheatCard() : CharTestCard(-1, CardType.Status, CardRarity.Status, TargetType.None)
{
    public override Texture2D? CustomFrame => ResourceLoader.Load<Texture2D>("res://images/sts2chartest/ui/custom_card_frames/overheat_power.png");
}