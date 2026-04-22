using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace StS2CharTest.Code.Character;

public class BlazeVar : DynamicVar
{
    private static  string name = "Blaze";
    
    public BlazeVar(decimal baseValue) : base(name, baseValue)
    {
        this.WithTooltip("BLAZE");
    }
}