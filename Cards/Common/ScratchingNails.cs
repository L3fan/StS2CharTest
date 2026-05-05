using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using StS2CharTest.Actions;
using StS2CharTest.Code.Character;

namespace StS2CharTest.Cards.Common;

[Pool(typeof(CharTestCardPool))]
public class ScratchingNails() : CharTestCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(2, ValueProp.Move), new RepeatVar(2), new HeatVar(1)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {  
        AttackCommand attackCommand = await CommonActions.CardAttack(this, play, DynamicVars.Repeat.IntValue).Execute(choiceContext);
        IEnumerator<DamageResult> results = attackCommand.Results.GetEnumerator();
        while (results.MoveNext())
        {
            if (results.Current.UnblockedDamage > 0)
                await CharTestActions.GainHeat(choiceContext, Owner, DynamicVars["Heat"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1);
    }
}