using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace StS2CharTest.CustomNodes;

public partial class CustomButton : NButton
{
    public override void _Ready()
    {
        ConnectSignals();
    }
}