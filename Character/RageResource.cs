using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;

namespace StS2CharTest.Code.Character;

public class RageResource
{
    public static readonly SpireField<PlayerCombatState, decimal> Amount = new(() => 0);
    public static readonly SpireField<PlayerCombatState, decimal> MaxAmount = new(() => 999);

    public static readonly SpireField<PlayerCombatState, Action<int,int>> RageChanged = new(() => null);

    public static readonly SpireField<NCombatUi, NRageCounter> _rageCounter = new(() => null);
    
    public static readonly SpireField<NCard, TextureRect> rageCostIcon = new(() => null);
    public static readonly SpireField<NCard, MegaLabel> rageCostLabel = new(() => null);
}