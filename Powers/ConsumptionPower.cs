using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace StS2CharTest.Powers;

public class ConsumptionPower : CharTestPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("MaxStack", 2)];

    public override async Task BeforePowerAmountChanged(PowerModel power, decimal amount, Creature target, Creature? applier,
        CardModel? cardSource)
    {
        if (power != this || !Owner.IsPlayer || amount <= 0)
            return;
        decimal newAmount = amount + Amount;
        int activateTimes = (int)Math.Floor(newAmount / DynamicVars["MaxStack"].IntValue);
        await PlayerCmd.GainEnergy(activateTimes, Owner.Player);
        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), activateTimes, Owner.Player);
        
        await PowerCmd.ModifyAmount(this, -DynamicVars["MaxStack"].IntValue * activateTimes, applier, cardSource);
    }
}