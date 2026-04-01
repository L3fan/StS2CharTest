using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using StS2CharTest.Code.Character;
using StS2CharTest.Code.Powers;
using StS2CharTest.Powers;

namespace StS2CharTest.Cards.Overheat;

[Pool(typeof(CharTestCardPool))]
public class BurnItDown() : OverheatCard(), OverheatPower.IChoosable
{
    public override int MaxUpgradeLevel => 0;
    
    public override bool CanBeGeneratedInCombat => false;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BurnItDownPower>(9)];

    public async Task OnChosen()
    {
        await CommonActions.Apply<BurnItDownPower>(Owner.Creature, this, DynamicVars["BurnItDownPower"].IntValue);
    }
}