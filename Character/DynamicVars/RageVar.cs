using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace StS2CharTest.Code.Character;

public class RageVar : DynamicVar
{
    private static  string name = "Rage";
    
    public RageVar(decimal baseValue) : base(name, baseValue)
    {
        this.WithTooltip("RAGE_COUNT");
    }
}