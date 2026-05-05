using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using StS2CharTest.Code.Relics;

namespace StS2CharTest.Powers;

public class EmbersPower : CharTestPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if(side == Owner.Side)
            await DealDamage();
    }

    public async Task DealDamage(bool reduceEmbers = true)
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner, CalculateTotalDamage(), ValueProp.Unpowered, null, null);
        if (Owner.IsAlive && reduceEmbers)
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(),this, (decimal)Math.Floor(-Amount/2f), null, null);
        else
            await Cmd.CustomScaledWait(0.1f, 0.25f);
    }

    public int CalculateTotalDamage()
    {
        bool takesOneDamage = Owner.HasPower<IntangiblePower>() || Owner.HasPower<SlipperyPower>();
        return (int)(takesOneDamage ? 1 : Amount);
    }

    public async Task TriggerBlazeDamage(Creature source)
    {
        MainFile.Logger.Info("Blaze Triggered");
        await DealDamage(false);

        await TriggerOnBlaze(source);
    }

    public async Task TriggerOnBlaze(Creature source)
    {
        foreach (AbstractModel iterateHookListener in CombatState.IterateHookListeners())
        {
            if (iterateHookListener.GetType().IsSubclassOf(typeof(CharTestPowerModel)))
            {
                CharTestPowerModel power = iterateHookListener as CharTestPowerModel;
                if (power.Owner == source)
                    await power.OnBlaze();
            }

            if (!source.IsPlayer)
                return;
            Player player = source.Player;
            if (iterateHookListener.GetType().IsSubclassOf(typeof(CharTestRelic)))
            {
                CharTestRelic relic = iterateHookListener as CharTestRelic;
                if (relic.Owner == player)
                    await relic.OnBlaze();
            }
        }
    }
}