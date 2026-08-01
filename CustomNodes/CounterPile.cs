using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace StS2CharTest.CustomNodes;

public class CounterPile : CardPile
{
    [CustomEnum] 
    public static PileType Counter;
    public CounterPile() : base(Counter)
    {
        
    }
}