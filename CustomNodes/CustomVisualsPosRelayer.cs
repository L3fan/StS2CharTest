using Godot;

namespace StS2CharTest.CustomNodes;

public partial class CustomVisualsPosRelayer : Node2D
{
    private float countDownTimer = 0.1f;

    public override void _Ready()
    {
        MainFile.Logger.Info("Player Visuals Pos: " + GlobalPosition);
    }
    
    public override void _Process(double delta)
    {
        if (countDownTimer > 0)
        {
            MainFile.Logger.Info("Player Visuals Pos: " + GlobalPosition);
            countDownTimer -= (float)delta;
        }
        else
        {
        }
    }
}