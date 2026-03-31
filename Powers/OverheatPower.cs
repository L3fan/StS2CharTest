using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2CharTest.Cards.Overheat;
using StS2CharTest.Powers;

namespace StS2CharTest.Code.Powers;

public class OverheatPower : CharTestPowerModel
{
    public interface IChoosable
    {
        Task OnChosen();
    }
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public IReadOnlyList<IReadOnlyList<IChoosable>> choosableOverheatPowers = new List<IReadOnlyList<IChoosable>>()
    {
        new List<IChoosable>()
        {
            ModelDb.Card<BurningWings>(),
            ModelDb.Card<BurnItDown>()
        }
    };

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        decimal newAmount = amount;
        if (newAmount + Amount > 3)
            return;
        await base.BeforeApplied(target, newAmount, applier, cardSource);
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Owner.IsDead || !Owner.IsPlayer)
            return;
        List<Task> list = new List<Task>();
        list.Add(ChooseOverheatPower());
        await Task.WhenAll(list);
    }

    public async Task ChooseOverheatPower()
    {
        List<CardModel> cards = choosableOverheatPowers[Amount-1].Select(delegate(IChoosable c)
        {
            CardModel cardModel = base.CombatState.CreateCard((CardModel)c, Owner.Player);
            return cardModel;
        }).ToList();
        await ((IChoosable)(await CardSelectCmd.FromChooseACardScreen(new BlockingPlayerChoiceContext(), cards, Owner.Player))).OnChosen();
    }
}