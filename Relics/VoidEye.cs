using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using StS2CharTest.Actions;
using StS2CharTest.Code.Character;

namespace StS2CharTest.Code.Relics;

[Pool(typeof(CharTestRelicPool))]
public class VoidEye() : CustomRelicModel
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override string PackedIconPath => "res://images/sts2chartest/relics/VoidEye.png";

    protected override string BigIconPath => "res://images/sts2chartest/relics/big/VoidEye.png";

    protected override string PackedIconOutlinePath => "res://images/sts2chartest/relics/outline/VoidEye.png";

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<SunStone>();
    }


    protected override IEnumerable<DynamicVar> CanonicalVars => [new RageVar( 3).WithTooltip("RAGE")];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Burn>()];

    public override async Task BeforeCombatStartLate()
    {
        Flash();
        await CharTestActions.GainRage(new BlockingPlayerChoiceContext(), Owner, DynamicVars["Rage"].IntValue);
    }

    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (card is not Burn burnCard)
            return Task.CompletedTask;
        
        burnCard.AddKeyword(CardKeyword.Ethereal);
        return Task.CompletedTask;
    }
}