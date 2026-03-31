using BaseLib.Abstracts;
using Godot;
using StS2CharTest.Cards;

namespace StS2CharTest.Code.Character;

public class OverheatCardPool : CharTestCardPool
{
    public override string Title => CharTest.CharacterId + "Overheat";
    
    public override Color DeckEntryCardColor => new("30C9C9");
    public override Color EnergyOutlineColor => new("26140b");
    public override bool IsColorless => false;
    //public override bool IsShared => true;

    public override string BigEnergyIconPath => "res://images/sts2chartest/ui/big_energy.png";
    public override string TextEnergyIconPath => "res://images/sts2chartest/ui/text_energy.png";

    public override float H => 180/355f;
    public override float S => 1.1f;
    public override float V => 1.0f;
}