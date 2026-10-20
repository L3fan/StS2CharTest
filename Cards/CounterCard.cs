using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2CharTest.Actions;
using StS2CharTest.CustomNodes;

namespace StS2CharTest.Cards;

public abstract class CounterCard(int cost, CardType cardType, CardRarity cardRarity, TargetType targetType) : CharTestCard(cost, cardType, cardRarity, targetType)
{
    public bool playedFromCounterPile = false;
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        //MainFile.Logger.Info("Card was played from Counter Pile: " + playedFromCounterPile);
        if (playedFromCounterPile)
            await OnCounterPlay(choiceContext, play);
        else
            await OnHandPlay(choiceContext, play);
    }

    protected virtual async Task OnCounterPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        
    }

    protected virtual async Task OnHandPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CharTestActions.AddToCounterPile(new CardModel[] { this }, Owner.Creature);
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (oldPileType == CounterPile.Counter)
            playedFromCounterPile = true;
        else
            playedFromCounterPile = false;
        
        return base.AfterCardChangedPiles(card, oldPileType, clonedBy);
    }
}