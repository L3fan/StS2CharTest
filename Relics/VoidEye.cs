using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using StS2CharTest.Code.Character;
using StS2CharTest.Code.Powers;

namespace StS2CharTest.Code.Relics;

[Pool(typeof(CharTestRelicPool))]
public class VoidEye() : CustomRelicModel
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override string PackedIconPath => "res://images/sts2chartest/relics/VoidEye.png";

    protected override string BigIconPath => "res://images/sts2chartest/relics/big/VoidEye.png";

    protected override string PackedIconOutlinePath => "res://images/sts2chartest/relics/outline/VoidEye.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new ("Heat", 1)];

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (Owner.Creature.Side != side)
            return;
        Flash();
        await PowerCmd.Apply<HeatPower>(Owner.Creature, DynamicVars["Heat"].BaseValue, Owner.Creature, null);
    }
}