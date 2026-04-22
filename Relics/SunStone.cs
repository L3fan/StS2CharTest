using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using StS2CharTest.Actions;
using StS2CharTest.Code.Character;
using StS2CharTest.Code.Powers;

namespace StS2CharTest.Code.Relics;

[Pool(typeof(CharTestRelicPool))]
public class SunStone() : CustomRelicModel
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;
    
    public override string PackedIconPath => "res://images/sts2chartest/relics/SunStone.png";

    protected override string BigIconPath => "res://images/sts2chartest/relics/big/SunStone.png";

    protected override string PackedIconOutlinePath => "res://images/sts2chartest/relics/outline/SunStone.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Heat", 10).WithTooltip("HEAT")];

    public override async Task BeforeCombatStartLate()
    {
        Flash();
        await CharTestActions.GainHeat(Owner, DynamicVars["Heat"].IntValue);
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        /*if (Owner.Creature.Side != side || Owner.Creature.CombatState.RoundNumber == 1)
            return;
        Flash();
        await PowerCmd.Apply<HeatPower>(Owner.Creature, 1, Owner.Creature, null);*/
    }

    
}