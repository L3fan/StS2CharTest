using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using StS2CharTest.Actions;
using StS2CharTest.Code.Powers;

namespace StS2CharTest.Cards.Dev;

[Pool(typeof(CurseCardPool))]
public class Gain20Heat() : CharTestCard(0, CardType.Skill,
    CardRarity.Status, TargetType.Self)
{
    public override int MaxUpgradeLevel => 0;
    
    public override bool CanBeGeneratedInCombat => false;
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CharTestActions.GainHeat(Owner, 20);
    }

    protected override void OnUpgrade()
    {

    }

    public override string CustomPortraitPath => "res://images/packed/card_portraits/Parrot.png";
    public override string PortraitPath => "res://images/packed/card_portraits/big/Parrot.png";
}