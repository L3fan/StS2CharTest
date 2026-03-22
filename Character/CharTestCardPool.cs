using BaseLib.Abstracts;
using Godot;

namespace StS2CharTest.Code.Character;

public class CharTestCardPool : CustomCardPoolModel
{
    public override string Title => CharTest.CharacterId;
    
    public override Color DeckEntryCardColor => new("ff5500");
    public override Color EnergyOutlineColor => new("26140b");
    public override bool IsColorless => false;

    public override string BigEnergyIconPath => "res://images/sts2chartest/ui/big_energy.png";
    public override string TextEnergyIconPath => "res://images/sts2chartest/ui/text_energy.png";

    public override float H => 20/355f;
    public override float S => 1.1f;
    public override float V => 1.0f;
}