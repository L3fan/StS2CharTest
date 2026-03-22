using BaseLib.Abstracts;
using Godot;

namespace StS2CharTest.Code.Character;

public class CharTestRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => CharTest.CharacterId;
    public override Color LabOutlineColor => CharTest.Color;
}