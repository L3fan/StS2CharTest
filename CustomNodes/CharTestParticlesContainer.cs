using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace StS2CharTest.CustomNodes;

public partial class CharTestParticlesContainer : Node2D
{
    private List<GpuParticles2D>? _particles;

    public override void _Ready()
    {
        _particles = new List<GpuParticles2D>();
        foreach (Node child in GetChildren())
        {
            if (child.GetType() != typeof(GpuParticles2D))
                continue;
            this._particles.Add(child as GpuParticles2D);
        }
    }
    public void SetEmitting(bool emitting)
    {
        for (int index = 0; index < this._particles.Count; ++index)
            _particles[index].Emitting = emitting;
    }

    public void Restart()
    {
        for (int index = 0; index < this._particles.Count; ++index)
            this._particles[index].Restart();
    }
}