using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;
using StS2CharTest.Code.Character;

namespace StS2CharTest.CustomNodes;

[GlobalClass]
public partial class CustomCreatureVisuals : NCreatureVisuals
{
    private AnimationPlayer? _animationPlayer;
    public override void _Ready()
    {
        base._Ready();
        _animationPlayer = GetNode<AnimationPlayer>("%Visuals/AnimationPlayer");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void _OnAnimationFinished(StringName animName)
    {
        if (_animationPlayer == null)
            return;
        
        if (animName != "idle" && animName != "die")
        {
            _animationPlayer.Play("idle");
        }
    }
}