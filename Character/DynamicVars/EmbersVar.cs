using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace StS2CharTest.Code.Character;

public class EmbersVar : DynamicVar
{
    private static  string name = "EmbersPower";
    
    public EmbersVar(decimal baseValue) : base(name, baseValue)
    {
        this.WithTooltip("EMBERS");
    }
}