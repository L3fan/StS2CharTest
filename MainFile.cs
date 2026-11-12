using System.Reflection;
using BaseLib.Patches.Features;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Modding;
using StS2CharTest.CustomNodes;

namespace StS2CharTest;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    private const string ModId = "StS2CharTest"; //At the moment, this is used only for the Logger and harmony names.

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);

        harmony.PatchAll(assembly);
    }
}