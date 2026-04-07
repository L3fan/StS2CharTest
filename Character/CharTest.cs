using System.Runtime.CompilerServices;
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using StS2CharTest.Code.Cards;
using StS2CharTest.Code.Relics;
using StS2CharTest.CustomNodes;

namespace StS2CharTest.Code.Character;

#pragma warning disable STS001
public class CharTest : CustomCharacterModel
#pragma warning restore STS001
{
    public const string CharacterId = "CharTest";

    public override List<string> GetArchitectAttackVfx()
    {
        return
        [
            "vfx/vfx_attack_blunt", "vfx/vfx_attack_slash", "vfx/vfx_attack_slash", "vfx/vfx_fire_burst",
            "vfx/vfx_fire_burst"
        ];
    }

    public static readonly Color Color = new(0.9f, 0.9f, 0.6f);
    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 70;
    public override int StartingGold => 99;
    public override CardPoolModel CardPool => ModelDb.CardPool<CharTestCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<CharTestRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<CharTestPotionPool>();

    public override float AttackAnimDelay => 0.2f;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeCharTest>(),
        ModelDb.Card<StrikeCharTest>(),
        ModelDb.Card<StrikeCharTest>(),
        ModelDb.Card<StrikeCharTest>(),
        ModelDb.Card<DefendCharTest>(),
        ModelDb.Card<DefendCharTest>(),
        ModelDb.Card<DefendCharTest>(),
        ModelDb.Card<DefendCharTest>(),
        ModelDb.Card<TailSwipe>(),
        ModelDb.Card<ShieldTheFlame>()
    ];

    public override string CustomAttackSfx {
        get
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 2);
            interpolatedStringHandler.AppendLiteral("event:/sfx/characters/");
            interpolatedStringHandler.AppendFormatted("ironclad");
            interpolatedStringHandler.AppendLiteral("/");
            interpolatedStringHandler.AppendFormatted("ironclad");
            interpolatedStringHandler.AppendLiteral("_attack");
            return interpolatedStringHandler.ToStringAndClear();
        }
    }

    public override string CustomCastSfx
    {
        get
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 2);
            interpolatedStringHandler.AppendLiteral("event:/sfx/characters/");
            interpolatedStringHandler.AppendFormatted("ironclad");
            interpolatedStringHandler.AppendLiteral("/");
            interpolatedStringHandler.AppendFormatted("ironclad");
            interpolatedStringHandler.AppendLiteral("_cast");
            return interpolatedStringHandler.ToStringAndClear();
        }
    }
    public override string CustomDeathSfx => "res://";
    public override string? CustomCharacterSelectBg => "res://scenes/sts2chartest/char_select_bg_chartest.tscn";

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<VoidEye>()];
    public override string CustomVisualPath => "res://scenes/sts2chartest/sts2chartest2.tscn";
    
    public override string CustomIconTexturePath => "res://images/sts2chartest/character_icon_chartest.png";
    public override string CustomCharacterSelectIconPath => "res://images/sts2chartest/char_select_chartest.png";
    public override string CustomCharacterSelectLockedIconPath => "res://images/sts2chartest/char_select_chartest_locked.png";
    public override string CustomMapMarkerPath => "res://images/sts2chartest/map_marker_chartest.png";

    public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/fade_transition_mat.tres";

    public override string CustomMerchantAnimPath => "res://scenes/sts2chartest/merchant_anim_chartest.tscn";

    public override string CustomEnergyCounterPath => "res://scenes/sts2chartest/energy_counter_chartest.tscn";
    public override string CustomRestSiteAnimPath => "res://scenes/sts2chartest/rest_site_anim_chartest.tscn";
    public override string CustomIconPath => "res://scenes/sts2chartest/CharTestIcon.tscn";

    public string CustomStaticHoverTipsPath => "res://StS2CharTest/localization/eng/static_hover_tips.json";
    
    public override Color EnergyLabelOutlineColor => new Color("8d5216");
}