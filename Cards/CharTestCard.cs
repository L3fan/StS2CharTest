using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Patches.Content;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using StS2CharTest.Code.Relics;
using StS2CharTest.Powers;

namespace StS2CharTest.Cards;

public abstract class CharTestCard(int cost, CardType cardType, CardRarity cardRarity, TargetType targetType) : CustomCardModel(cost,
    cardType, cardRarity,
    targetType), CharTestModel
{
    public override string CustomPortraitPath => "res://images/packed/card_portraits/" + Id.Entry.RemovePrefix().ToLowerInvariant() + ".png";
    public override string PortraitPath => "res://images/packed/card_portraits/big/" + Id.Entry.RemovePrefix().ToLowerInvariant() + ".png";

    private bool _rageCostSet;
    private int _baseRageCost;
    public virtual int CanonicalRageCost => -1;

    public int BaseRageCost
    {
        get
        {
            if (!this.IsMutable)
                return this.CanonicalRageCost;
            if (!this._rageCostSet)
            {
                this._baseRageCost = this.CanonicalRageCost;
                this._rageCostSet = true;
            }
            return this._baseRageCost;
        }
        private set
        {
            this.AssertMutable();
            if (!this.HasRageCostX)
            {
                this._baseRageCost = value;
                this._rageCostSet = true;
            }
            Action rageCostChanged = this.RageCostChanged;
            if (rageCostChanged == null)
                return;
            rageCostChanged();
        }
    }
    
    public bool HasChangedRageCost { get {return CanonicalRageCost != _baseRageCost;} }
    
    private bool _wasRageCostJustUpgraded;
    public bool WasRageCostJustUpgraded => this._wasRageCostJustUpgraded;
    
    private List<TemporaryCardCost> _temporaryRageCosts = new List<TemporaryCardCost>();
    
    public TemporaryCardCost? TemporaryRageCost
    {
        get => this._temporaryRageCosts.LastOrDefault<TemporaryCardCost>();
    }
    
    private int _lastRageSpent;
    
    public event Action? RageCostChanged;
    
    public virtual int CurrentRageCost
    {
        get
        {
            int? cost = this._temporaryRageCosts.LastOrDefault<TemporaryCardCost>()?.Cost;
            if (!cost.HasValue)
                return this.BaseRageCost;
            int? nullable = cost;
            int num = 0;
            return nullable.GetValueOrDefault() == num & nullable.HasValue && this.BaseRageCost < 0 ? this.BaseRageCost : cost.Value;
        }
    }

    public virtual bool HasRageCostX => false;
    
    public int LastRageSpent
    {
        get => this._lastRageSpent;
        set
        {
            this.AssertMutable();
            this._lastRageSpent = value;
        }
    }

    public int ResolveRageXValue()
    {
        if (!this.HasRageCostX)
            throw new InvalidOperationException("This card does not have an X-cost.");
        return Hook.ModifyXValue(this.CombatState, this, this.LastRageSpent);
    }

    protected override void DeepCloneFields()
    {
        base.DeepCloneFields();
        _temporaryRageCosts = _temporaryRageCosts.ToList<TemporaryCardCost>();
    }

    protected override void AfterCloned()
    {
        base.AfterCloned();
        this.RageCostChanged = (Action) null;
    }

    public void SetRageCostUntilPlayed(int cost)
    {
        this.AddTemporaryRageCost(TemporaryCardCost.UntilPlayed(cost));
    }

    public void SetRageCostThisTurn(int cost)
    {
        AddTemporaryRageCost(TemporaryCardCost.ThisTurn(cost));
    }

    public void SetRageCostThisCombat(int cost)
    {
        this.AddTemporaryRageCost(TemporaryCardCost.ThisCombat(cost));
    }
    
    public int GetRageCostThisCombat()
    {
        TemporaryCardCost temporaryCardCost = this._temporaryRageCosts.FirstOrDefault<TemporaryCardCost>((Func<TemporaryCardCost, bool>) (cost => cost != null && !cost.ClearsWhenTurnEnds && !cost.ClearsWhenCardIsPlayed));
        return temporaryCardCost == null ? this.BaseRageCost : temporaryCardCost.Cost;
    }

    private void AddTemporaryRageCost(TemporaryCardCost cost)
    {
        this.AssertMutable();
        this._temporaryRageCosts.Add(cost);
        Action rageCostChanged = RageCostChanged;
        if (rageCostChanged == null)
            return;
        rageCostChanged();
    }
    
    protected void UpgradeRageCostBy(int addend)
    {
        if (this.HasRageCostX)
            throw new InvalidOperationException($"UpgradeRageCostBy called on {this.Id.Entry} which has rage cost X.");
        if (addend == 0)
            return;
        int baseRageCost = this.BaseRageCost;
        this.BaseRageCost += addend;
        this._wasRageCostJustUpgraded = true;
        if (this.BaseRageCost >= baseRageCost)
            return;
        this._temporaryRageCosts.RemoveAll((Predicate<TemporaryCardCost>) (c => c.Cost > this.BaseRageCost));
    }
    
    public int GetRageCostWithModifiers()
    {
        if (this.HasRageCostX)
        {
            PlayerCombatState playerCombatState = this.Owner.PlayerCombatState;
            return playerCombatState == null ? 0 : (int)RageResource.Amount.Get(playerCombatState);
        }
        CardPile pile = this.Pile;
        return pile != null && pile.IsCombatPile && this.CombatState != null ? (int) ModifyRageCost(this.CombatState, this, (Decimal) this.CurrentRageCost) : this.CurrentRageCost;
    }
    
    public static Decimal ModifyRageCost(ICombatState combatState, CardModel card, Decimal originalCost)
    {
        if (originalCost < 0M)
            return originalCost;
        Decimal modifiedCost = originalCost;
        foreach (AbstractModel iterateHookListener in combatState.IterateHookListeners())
        {
            if (!iterateHookListener.GetType().IsSubclassOf(typeof(CharTestCard)))
                continue;
            CharTestCard charTestCard = iterateHookListener as CharTestCard;
            charTestCard.TryModifyRageCost(card, modifiedCost, out modifiedCost);
        }

        return modifiedCost;
    }

    public bool TryModifyRageCost(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        modifiedCost = originalCost;
        return false;
    }
    
    public async Task SpendRage(int amount)
    {
        this.LastRageSpent = amount;
        if (amount <= 0 || RageResource.Amount.Get(Owner.PlayerCombatState) == 0)
            return;
        decimal currentRage = RageResource.Amount.Get(Owner.PlayerCombatState);
        if (currentRage < LastRageSpent)
            LastRageSpent = (int)RageResource.Amount.Get(Owner.PlayerCombatState);
        RageResource.Amount.Set(Owner.PlayerCombatState, currentRage-(Decimal)LastRageSpent);
        RageResource.RageChanged.Get(Owner.PlayerCombatState).Invoke((int)currentRage, (int)RageResource.Amount.Get(Owner.PlayerCombatState));
        await AfterRageSpent(this.Owner.Creature.CombatState, LastRageSpent, this.Owner);
    }

    private async Task AfterRageSpent(ICombatState? combatState, int amount, Player spender)
    {
        foreach (AbstractModel model in combatState.IterateHookListeners())
        {
            if (model is CharTestModel)
                continue;
            CharTestModel charTestModel = model as CharTestModel;
            await charTestModel.AfterRageSpent(amount, spender);
            model.InvokeExecutionFinished();
        }
    }
    
    public void EndOfTurnCleanupRage()
    {
        if (_temporaryRageCosts.RemoveAll((Predicate<TemporaryCardCost>) (c => c.ClearsWhenTurnEnds)) <= 0)
            return;
        Action rageCostChanged = this.RageCostChanged;
        if (rageCostChanged == null)
            return;
        rageCostChanged();
    }
    
    private IReadOnlyList<Creature> GetPossibleTargets()
    {
        return CombatState.GetOpponentsOf(Owner.Creature);
    }

    [CustomEnum]
    public static CardType Overrage;

    public void ResetTempRageCost()
    {
        if (_temporaryRageCosts.RemoveAll((Predicate<TemporaryCardCost>)(c => c.ClearsWhenCardIsPlayed)) >
            0)
        {
            Action rageCostChanged = RageCostChanged;
            if (rageCostChanged != null)
                rageCostChanged();
        }
    }

    public void SetRageCostWasJustUpgraded(bool b)
    {
        _wasRageCostJustUpgraded = b;
    }

    public void DowngradeRageCost(CharTestCard mutable)
    {
        this._baseRageCost = mutable.CanonicalRageCost;
    }
}

public class RageResource
{
    public static readonly SpireField<PlayerCombatState, decimal> Amount = new(() => 0);
    public static readonly SpireField<PlayerCombatState, decimal> MaxAmount = new(() => 20);

    public static readonly SpireField<PlayerCombatState, Action<int,int>> RageChanged = new(() => null);

    public static readonly SpireField<CharacterModel, bool> ShouldAlwaysShowRageCounter = new(() => false);
    public static readonly SpireField<NCombatUi, NRageCounter> _rageCounter = new(() => null);
    
    public static readonly SpireField<NCard, TextureRect> rageCostIcon = new(() => null);
    public static readonly SpireField<NCard, MegaLabel> rageCostLabel = new(() => null);
}

public interface CharTestModel
{
    public virtual Task AfterRageSpent(int amount, Player spender)
    {
        return Task.CompletedTask;
    }
    
    public virtual Task AfterRageGained(PlayerChoiceContext choiceContext, int amount, Player gainer)
    {
        return Task.CompletedTask;
    }
}