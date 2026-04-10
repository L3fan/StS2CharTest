using System.Reflection;
using Godot;
using Godot.Collections;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace StS2CharTest.CustomNodes;

public partial class CustomParticlesContainer : NParticlesContainer
{
    private static readonly FieldInfo? ParticlesField = AccessTools.Field(typeof (NParticlesContainer), "_particles");

    public override void _Ready()
    {
        Array<GpuParticles2D> particles = new Array<GpuParticles2D>();
        foreach (Node child in GetChildren())
        {
            if(child is GpuParticles2D)
                particles.Add(child as GpuParticles2D);
        }
        ParticlesField?.SetValue(this, particles);
    }
}