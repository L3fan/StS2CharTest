using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace StS2CharTest.Powers;

public class EmbersPower : CharTestPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
            return;
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner, Amount, ValueProp.Unpowered, null, null);
        if (Owner.IsAlive)
            await PowerCmd.ModifyAmount(this, (decimal)Math.Floor(-Amount/2f), null, null);
        else
            await Cmd.CustomScaledWait(0.1f, 0.25f);
    }

    public override async Task OnBlazeTriggered()
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner, Amount, ValueProp.Unpowered, null, null);
        if (Owner.IsAlive)
            await PowerCmd.ModifyAmount(this, (decimal)Math.Floor(-Amount/2f), null, null);
        else
            await Cmd.CustomScaledWait(0.1f, 0.25f);
    }

    public int CalculateTotalDamageNextTurn()
    {
        return Amount;
    }
}