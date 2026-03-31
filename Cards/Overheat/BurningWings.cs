using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using StS2CharTest.Code.Character;
using StS2CharTest.Code.Powers;
using StS2CharTest.Powers;

namespace StS2CharTest.Cards.Overheat;

[Pool(typeof(CharTestCardPool))]
public class BurningWings() : CharTestCard(-1,
    CardType.Status, CardRarity.Status,
    TargetType.None), OverheatPower.IChoosable
{
    public override int MaxUpgradeLevel => 0;
    
    public override bool CanBeGeneratedInCombat => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FlyingPower>(1).WithTooltip("FLYING")];

    public async Task OnChosen()
    {
        await CommonActions.Apply<FlyingPower>(Owner.Creature, this, DynamicVars["FlyingPower"].IntValue);
    }

    public override Color CustomBackgroundColor => new("30C9C9");
}