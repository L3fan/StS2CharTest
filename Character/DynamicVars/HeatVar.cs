using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace StS2CharTest.Code.Character;

public class HeatVar : DynamicVar
{
    private static  string name = "Heat";
    
    public HeatVar(decimal baseValue) : base(name, baseValue)
    {
        this.WithTooltip("HEAT_COUNT");
    }
}