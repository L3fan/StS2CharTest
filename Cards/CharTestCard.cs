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

    private bool _heatCostSet;
    private int _baseHeatCost;
    public virtual int CanonicalHeatCost => -1;

    public int BaseHeatCost
    {
        get
        {
            if (!this.IsMutable)
                return this.CanonicalHeatCost;
            if (!this._heatCostSet)
            {
                this._baseHeatCost = this.CanonicalHeatCost;
                this._heatCostSet = true;
            }
            return this._baseHeatCost;
        }
        private set
        {
            this.AssertMutable();
            if (!this.HasHeatCostX)
            {
                this._baseHeatCost = value;
                this._heatCostSet = true;
            }
            Action heatCostChanged = this.HeatCostChanged;
            if (heatCostChanged == null)
                return;
            heatCostChanged();
        }
    }
    
    public bool HasChangedHeatCost { get {return CanonicalHeatCost != _baseHeatCost;} }
    
    private bool _wasHeatCostJustUpgraded;
    public bool WasHeatCostJustUpgraded => this._wasHeatCostJustUpgraded;
    
    private List<TemporaryCardCost> _temporaryHeatCosts = new List<TemporaryCardCost>();
    
    public TemporaryCardCost? TemporaryHeatCost
    {
        get => this._temporaryHeatCosts.LastOrDefault<TemporaryCardCost>();
    }
    
    private int _lastHeatSpent;
    
    public event Action? HeatCostChanged;
    
    public virtual int CurrentHeatCost
    {
        get
        {
            int? cost = this._temporaryHeatCosts.LastOrDefault<TemporaryCardCost>()?.Cost;
            if (!cost.HasValue)
                return this.BaseHeatCost;
            int? nullable = cost;
            int num = 0;
            return nullable.GetValueOrDefault() == num & nullable.HasValue && this.BaseHeatCost < 0 ? this.BaseHeatCost : cost.Value;
        }
    }

    public virtual bool HasHeatCostX => false;
    
    public int LastHeatSpent
    {
        get => this._lastHeatSpent;
        set
        {
            this.AssertMutable();
            this._lastHeatSpent = value;
        }
    }

    public int ResolveHeatXValue()
    {
        if (!this.HasHeatCostX)
            throw new InvalidOperationException("This card does not have an X-cost.");
        return Hook.ModifyXValue(this.CombatState, this, this.LastHeatSpent);
    }

    protected override void DeepCloneFields()
    {
        base.DeepCloneFields();
        _temporaryHeatCosts = _temporaryHeatCosts.ToList<TemporaryCardCost>();
    }

    protected override void AfterCloned()
    {
        base.AfterCloned();
        this.HeatCostChanged = (Action) null;
    }

    public void SetHeatCostUntilPlayed(int cost)
    {
        this.AddTemporaryHeatCost(TemporaryCardCost.UntilPlayed(cost));
    }

    public void SetHeatCostThisTurn(int cost)
    {
        AddTemporaryHeatCost(TemporaryCardCost.ThisTurn(cost));
    }

    public void SetHeatCostThisCombat(int cost)
    {
        this.AddTemporaryHeatCost(TemporaryCardCost.ThisCombat(cost));
    }
    
    public int GetHeatCostThisCombat()
    {
        TemporaryCardCost temporaryCardCost = this._temporaryHeatCosts.FirstOrDefault<TemporaryCardCost>((Func<TemporaryCardCost, bool>) (cost => cost != null && !cost.ClearsWhenTurnEnds && !cost.ClearsWhenCardIsPlayed));
        return temporaryCardCost == null ? this.BaseHeatCost : temporaryCardCost.Cost;
    }

    private void AddTemporaryHeatCost(TemporaryCardCost cost)
    {
        this.AssertMutable();
        this._temporaryHeatCosts.Add(cost);
        Action heatCostChanged = HeatCostChanged;
        if (heatCostChanged == null)
            return;
        heatCostChanged();
    }
    
    protected void UpgradeHeatCostBy(int addend)
    {
        if (this.HasHeatCostX)
            throw new InvalidOperationException($"UpgradeHeatCostBy called on {this.Id.Entry} which has heat cost X.");
        if (addend == 0)
            return;
        int baseHeatCost = this.BaseHeatCost;
        this.BaseHeatCost += addend;
        this._wasHeatCostJustUpgraded = true;
        if (this.BaseHeatCost >= baseHeatCost)
            return;
        this._temporaryHeatCosts.RemoveAll((Predicate<TemporaryCardCost>) (c => c.Cost > this.BaseHeatCost));
    }
    
    public int GetHeatCostWithModifiers()
    {
        if (this.HasHeatCostX)
        {
            PlayerCombatState playerCombatState = this.Owner.PlayerCombatState;
            return playerCombatState == null ? 0 : (int)HeatResource.Amount.Get(playerCombatState);
        }
        CardPile pile = this.Pile;
        return pile != null && pile.IsCombatPile && this.CombatState != null ? (int) ModifyHeatCost(this.CombatState, this, (Decimal) this.CurrentHeatCost) : this.CurrentHeatCost;
    }
    
    public static Decimal ModifyHeatCost(CombatState combatState, CardModel card, Decimal originalCost)
    {
        if (originalCost < 0M)
            return originalCost;
        Decimal modifiedCost = originalCost;
        foreach (AbstractModel iterateHookListener in combatState.IterateHookListeners())
        {
            if (!iterateHookListener.GetType().IsSubclassOf(typeof(CharTestCard)))
                continue;
            CharTestCard charTestCard = iterateHookListener as CharTestCard;
            charTestCard.TryModifyHeatCost(card, modifiedCost, out modifiedCost);
        }

        return modifiedCost;
    }

    public bool TryModifyHeatCost(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        modifiedCost = originalCost;
        return false;
    }
    
    public async Task SpendHeat(int amount)
    {
        this.LastHeatSpent = amount;
        if (amount <= 0 || HeatResource.Amount.Get(Owner.PlayerCombatState) == 0)
            return;
        decimal currentHeat = HeatResource.Amount.Get(Owner.PlayerCombatState);
        if (currentHeat < LastHeatSpent)
            LastHeatSpent = (int)HeatResource.Amount.Get(Owner.PlayerCombatState);
        HeatResource.Amount.Set(Owner.PlayerCombatState, currentHeat-(Decimal)LastHeatSpent);
        HeatResource.HeatChanged.Get(Owner.PlayerCombatState).Invoke((int)currentHeat, (int)HeatResource.Amount.Get(Owner.PlayerCombatState));
        await AfterHeatSpent(this.Owner.Creature.CombatState, LastHeatSpent, this.Owner);
    }

    private async Task AfterHeatSpent(CombatState? combatState, int amount, Player spender)
    {
        foreach (AbstractModel model in combatState.IterateHookListeners())
        {
            if (model.GetType().IsSubclassOf(typeof(CharTestModel)))
                return;
            CharTestModel charTestModel = model as CharTestModel;
            await charTestModel.AfterHeatSpent(amount, spender);
            model.InvokeExecutionFinished();
        }
    }
    
    public void EndOfTurnCleanupHeat()
    {
        if (_temporaryHeatCosts.RemoveAll((Predicate<TemporaryCardCost>) (c => c.ClearsWhenTurnEnds)) <= 0)
            return;
        Action heatCostChanged = this.HeatCostChanged;
        if (heatCostChanged == null)
            return;
        heatCostChanged();
    }
    
    
    public async Task Blaze(Player source, int triggerAmount = 1, bool reduceEmbers = false)
    {
        await TriggerEmbers(source.Creature, triggerAmount, reduceEmbers, true);
    }

    public async Task Blaze(Creature source, int triggerAmount = 1, bool reduceEmbers = false)
    {
        await TriggerEmbers(source, triggerAmount, reduceEmbers, true);
    }

    public async Task TriggerEmbers(Creature source, int triggerAmount = 1, bool reduceEmbers = true, bool isBlaze = false)
    {
        for (int i = 0; i < triggerAmount; i++)
        {
            foreach (AbstractModel iterateHookListener in CombatState.IterateHookListeners())
            {
                if (!iterateHookListener.GetType().IsSubclassOf(typeof(EmbersPower)))
                    continue;
                EmbersPower embers = iterateHookListener as EmbersPower;
                Creature owner = embers.Owner;
                if (!owner.IsDead)
                {
                    if (owner.IsAlive)
                    {
                        await embers.TriggerDamage(source);
                    }
                }
            }
        }
    }
    
    private IReadOnlyList<Creature> GetPossibleTargets()
    {
        return CombatState.GetOpponentsOf(Owner.Creature);
    }

    [CustomEnum]
    public static CardType Overheat;

    public void ResetTempHeatCost()
    {
        if (_temporaryHeatCosts.RemoveAll((Predicate<TemporaryCardCost>)(c => c.ClearsWhenCardIsPlayed)) >
            0)
        {
            Action heatCostChanged = HeatCostChanged;
            if (heatCostChanged != null)
                heatCostChanged();
        }
    }

    public void SetHeatCostWasJustUpgraded(bool b)
    {
        _wasHeatCostJustUpgraded = b;
    }

    public void DowngradeHeatCost(CharTestCard mutable)
    {
        this._baseHeatCost = mutable.CanonicalHeatCost;
    }
}

public class HeatResource
{
    public static readonly SpireField<PlayerCombatState, decimal> Amount = new(() => 0);
    public static readonly SpireField<PlayerCombatState, decimal> MaxAmount = new(() => 20);

    public static readonly SpireField<PlayerCombatState, Action<int,int>> HeatChanged = new(() => null);

    public static readonly SpireField<CharacterModel, bool> ShouldAlwaysShowHeatCounter = new(() => false);
    public static readonly SpireField<NCombatUi, NHeatCounter> _heatCounter = new(() => null);
    
    public static readonly SpireField<NCard, TextureRect> heatCostIcon = new(() => null);
    public static readonly SpireField<NCard, MegaLabel> heatCostLabel = new(() => null);
}

public interface CharTestModel
{
    public virtual Task AfterHeatSpent(int amount, Player spender)
    {
        return Task.CompletedTask;
    }
    
    public virtual Task AfterHeatGained(int amount, Player gainer)
    {
        return Task.CompletedTask;
    }
}