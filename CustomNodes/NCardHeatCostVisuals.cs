using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Helpers.Models;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using StS2CharTest.Cards;

namespace StS2CharTest.CustomNodes;

public static class NCardHeatCostVisuals
{
    public static void UpdateHeatCostVisuals(NCard cardNode, PileType pileType, bool pretendCardCanBePlayed)
    {
        MegaLabel heatLabel = HeatResource.heatCostLabel.Get(cardNode);
        TextureRect heatIcon = HeatResource.heatCostIcon.Get(cardNode);

        if (!cardNode.Model.GetType().IsSubclassOf(typeof(CharTestCard)))
        {
            heatIcon.Visible = false;
            return;
        }
        
        CharTestCard charTestCard = cardNode.Model as CharTestCard;
        
        if (cardNode.Visibility != ModelVisibility.Visible)
        {
            heatLabel.SetTextAutoSize(string.Empty);
            heatIcon.Visible = false;
            heatLabel.AddThemeColorOverride(ThemeConstants.Label.FontColor, StsColors.cream);
            heatLabel.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, cardNode.Model.Pool.EnergyOutlineColor);
        }
        else
        {
            if (charTestCard.HasHeatCostX)
            {
                heatLabel.SetTextAutoSize("X");
                heatIcon.Visible = true;
            }
            else
            {
                heatLabel.SetTextAutoSize(charTestCard.GetHeatCostWithModifiers().ToString());
                heatIcon.Visible = charTestCard.GetHeatCostWithModifiers() >= 0;
            }

            UpdateHeatCostColor(cardNode, pileType, pretendCardCanBePlayed);
        }
    }

    private static void UpdateHeatCostColor(NCard cardNode, PileType pileType, bool pretendCardCanBePlayed)
    {
        CharTestCard charTestCard = cardNode.Model as CharTestCard;
        
        MegaLabel heatLabel = HeatResource.heatCostLabel.Get(cardNode);
        Color color1 = StsColors.cream;
        Color color2 = new ("b60020");
        if (!charTestCard.HasHeatCostX && charTestCard.WasHeatCostJustUpgraded)
        {
            color1 = StsColors.green;
            color2 = StsColors.energyGreenOutline;
        }
        else if (pileType == PileType.Hand)
        {
            CardCostColor heatCostColor = GetHeatCostColor(cardNode.Model, cardNode.Model.CombatState);
            color1 = GetCostTextColorInHand(heatCostColor, pretendCardCanBePlayed, color1);
            color2 = GetCostOutlineColorInHand(heatCostColor, pretendCardCanBePlayed, color2);
        }
        heatLabel.AddThemeColorOverride(ThemeConstants.Label.FontColor, color1);
        heatLabel.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, color2);
    }

    private static Color GetCostTextColorInHand(CardCostColor costColor, bool pretendCardCanBePlayed, Color defaultColor)
    {
        switch (costColor)
        {
            case CardCostColor.Unmodified:
                return defaultColor;
            case CardCostColor.Increased:
                return StsColors.energyBlue;
            case CardCostColor.Decreased:
                return StsColors.green;
            case CardCostColor.InsufficientResources:
                return pretendCardCanBePlayed ? defaultColor : StsColors.red;
            default:
                throw new ArgumentOutOfRangeException(nameof (costColor), (object) costColor, (string) null);
        }
    }
    
    private static Color GetCostOutlineColorInHand(CardCostColor costColor, bool pretendCardCanBePlayed, Color defaultColor)
    {
        switch (costColor)
        {
            case CardCostColor.Unmodified:
                return defaultColor;
            case CardCostColor.Increased:
                return StsColors.energyBlueOutline;
            case CardCostColor.Decreased:
                return StsColors.energyGreenOutline;
            case CardCostColor.InsufficientResources:
                return pretendCardCanBePlayed ? defaultColor : StsColors.unplayableEnergyCostOutline;
            default:
                throw new ArgumentOutOfRangeException(nameof (costColor), (object) costColor, (string) null);
        }
    }

    private static bool TryModifyHeatCostWithHooks(
        CardModel card,
        CombatState state,
        out Decimal hookModifiedCost)
    {
        hookModifiedCost = 0;
        if (!card.GetType().IsSubclassOf(typeof(CharTestCard)))
            return false;
        CharTestCard charTestCard = card as CharTestCard;
        hookModifiedCost = (Decimal) charTestCard.BaseHeatCost;
        bool flag = false;
        foreach (AbstractModel iterateHookListener in state.IterateHookListeners())
        {
            if (!iterateHookListener.GetType().IsSubclassOf(typeof(CharTestCard)))
                continue;
            CharTestCard charTestModel = iterateHookListener as CharTestCard;
            flag |= charTestModel.TryModifyHeatCost(card, hookModifiedCost, out hookModifiedCost);
        }

        return flag;
    }
    
    
    public static CardCostColor GetHeatCostColor(CardModel card, CombatState? state)
    {
        if (!card.GetType().IsSubclassOf(typeof(CharTestCard)))
            return CardCostColor.Unmodified;
        CharTestCard charTestCard = card as CharTestCard;
        if (state == null)
            return CardCostColor.Unmodified;
        if (charTestCard.HasHeatCostX)
            return CardCostColor.Unmodified;
        Decimal hookModifiedCost;
        if (TryModifyHeatCostWithHooks(charTestCard, state, out hookModifiedCost))
            return GetColorForHookModifiedCost(hookModifiedCost, charTestCard.BaseHeatCost);
        return charTestCard.TemporaryHeatCost != null ? GetColorForLocalCost(charTestCard.TemporaryHeatCost.Cost, charTestCard.BaseHeatCost) : CardCostColor.Unmodified;
    }
    

    private static CardCostColor GetColorForHookModifiedCost(Decimal hookModifiedCost, int baseCost)
    {
        if (hookModifiedCost > (Decimal) baseCost)
            return CardCostColor.Increased;
        return hookModifiedCost < (Decimal) baseCost ? CardCostColor.Decreased : CardCostColor.Unmodified;
    }
    

    private static CardCostColor GetColorForLocalCost(int localCost, int baseCost)
    {
        if (localCost > baseCost)
            return CardCostColor.Increased;
        return localCost < baseCost ? CardCostColor.Decreased : CardCostColor.Unmodified;
    }
}