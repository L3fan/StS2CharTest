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

public static class NCardRageCostVisuals
{
    public static void UpdateRageCostVisuals(NCard cardNode, PileType pileType, bool pretendCardCanBePlayed)
    {
        MegaLabel rageLabel = RageResource.rageCostLabel.Get(cardNode);
        TextureRect rageIcon = RageResource.rageCostIcon.Get(cardNode);

        if (!cardNode.Model.GetType().IsSubclassOf(typeof(CharTestCard)))
        {
            rageIcon.Visible = false;
            return;
        }
        
        CharTestCard charTestCard = cardNode.Model as CharTestCard;
        
        if (cardNode.Visibility != ModelVisibility.Visible)
        {
            rageLabel.SetTextAutoSize(string.Empty);
            rageIcon.Visible = false;
            rageLabel.AddThemeColorOverride(ThemeConstants.Label.FontColor, StsColors.cream);
            rageLabel.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, cardNode.Model.Pool.EnergyOutlineColor);
        }
        else
        {
            if (charTestCard.HasRageCostX)
            {
                rageLabel.SetTextAutoSize("X");
                rageIcon.Visible = true;
            }
            else
            {
                rageLabel.SetTextAutoSize(charTestCard.GetRageCostWithModifiers().ToString());
                rageIcon.Visible = charTestCard.GetRageCostWithModifiers() >= 0;
            }

            UpdateRageCostColor(cardNode, pileType, pretendCardCanBePlayed);
        }
    }

    private static void UpdateRageCostColor(NCard cardNode, PileType pileType, bool pretendCardCanBePlayed)
    {
        CharTestCard charTestCard = cardNode.Model as CharTestCard;
        
        MegaLabel rageLabel = RageResource.rageCostLabel.Get(cardNode);
        Color color1 = StsColors.cream;
        Color color2 = new ("b60020");
        if (!charTestCard.HasRageCostX && charTestCard.WasRageCostJustUpgraded)
        {
            color1 = StsColors.green;
            color2 = StsColors.energyGreenOutline;
        }
        else if (pileType == PileType.Hand)
        {
            CardCostColor rageCostColor = GetRageCostColor(cardNode.Model, cardNode.Model.CombatState);
            color1 = GetCostTextColorInHand(rageCostColor, pretendCardCanBePlayed, color1);
            color2 = GetCostOutlineColorInHand(rageCostColor, pretendCardCanBePlayed, color2);
        }
        rageLabel.AddThemeColorOverride(ThemeConstants.Label.FontColor, color1);
        rageLabel.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, color2);
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

    private static bool TryModifyRageCostWithHooks(
        CardModel card,
        ICombatState state,
        out Decimal hookModifiedCost)
    {
        hookModifiedCost = 0;
        if (!card.GetType().IsSubclassOf(typeof(CharTestCard)))
            return false;
        CharTestCard charTestCard = card as CharTestCard;
        hookModifiedCost = (Decimal) charTestCard.BaseRageCost;
        bool flag = false;
        foreach (AbstractModel iterateHookListener in state.IterateHookListeners())
        {
            if (!iterateHookListener.GetType().IsSubclassOf(typeof(CharTestCard)))
                continue;
            CharTestCard charTestModel = iterateHookListener as CharTestCard;
            flag |= charTestModel.TryModifyRageCost(card, hookModifiedCost, out hookModifiedCost);
        }

        return flag;
    }
    
    
    public static CardCostColor GetRageCostColor(CardModel card, ICombatState state)
    {
        if (!card.GetType().IsSubclassOf(typeof(CharTestCard)))
            return CardCostColor.Unmodified;
        CharTestCard charTestCard = card as CharTestCard;
        if (state == null)
            return CardCostColor.Unmodified;
        if (charTestCard.HasRageCostX)
            return CardCostColor.Unmodified;
        Decimal hookModifiedCost;
        if (TryModifyRageCostWithHooks(charTestCard, state, out hookModifiedCost))
            return GetColorForHookModifiedCost(hookModifiedCost, charTestCard.BaseRageCost);
        return charTestCard.TemporaryRageCost != null ? GetColorForLocalCost(charTestCard.TemporaryRageCost.Cost, charTestCard.BaseRageCost) : CardCostColor.Unmodified;
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