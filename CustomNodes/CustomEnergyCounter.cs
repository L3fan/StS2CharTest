using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace StS2CharTest.Character;

public partial class CustomEnergyCounter : NEnergyCounter
{
    public override void _Ready()
    {
        base._Ready();
        Control Layers = GetNode<Control>("%Layers");

        AddMaterialToChildrenOf(Layers);
    }

    private void AddMaterialToChildrenOf(Node nodeToCheck)
    {
        foreach (var child in nodeToCheck.GetChildren())
        {
            if (child.GetType() != typeof(TextureRect))
            {
                AddMaterialToChildrenOf(child);
                continue;
            }
            Control controlNode = child as Control;
            controlNode.Material = PreloadManager.Cache.GetMaterial("res://materials/ui/energy_orb_dark.tres");
        }
    }
}