using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.TestSupport;

namespace StS2CharTest.CustomNodes;

public partial class NCounterCardHolder : NCardHolder
{
    private bool _showHoverTips;
    private bool _scaleOnHover;
    private Vector2 _originalScale = Vector2.One;
    protected override Vector2 HoverScale => _originalScale * 1.1f;

    public override Vector2 SmallScale => _originalScale;

    public override bool IsShowingUpgradedCard
    {
        get
        {
            if (base.IsShowingUpgradedCard)
                return true;
            return CardModel != null && CardModel.UpgradePreviewType.IsPreview();
        }
    }

    private static string ScenePath => SceneHelper.GetScenePath("sts2chartest/counter_card_holder");

    public static IEnumerable<string> AssetPaths
    {
        get
        {
            return (IEnumerable<string>)new List<string>() { ScenePath };
        }
    }

    public static NCounterCardHolder? Create(NCard card, bool showHoverTips, bool scaleOnHover)
    {
        if (TestMode.IsOn)
            return (NCounterCardHolder)null;
        NCounterCardHolder nCounterCardHolder = PreloadManager.Cache.GetScene(ScenePath)
            .Instantiate<NCounterCardHolder>();
        nCounterCardHolder.Initialize(card, showHoverTips, scaleOnHover);
        return nCounterCardHolder;
    }

    public override void _Ready() => ConnectSignals();

    private void Initialize(NCard card, bool showHoverTips, bool scaleOnHover)
    {
        Name = (StringName)$"CounterCardHolder-{card.Model?.Id}";
        SetCard(card);
        Scale = SmallScale;
        _showHoverTips = showHoverTips;
        _scaleOnHover = scaleOnHover;
    }

    protected override void OnFocus()
    {
        _isHovered = true;
        if (_scaleOnHover)
        {
            _hoverTween?.Kill();
            Scale = HoverScale;
        }

        if (!_showHoverTips)
            return;
        CreateHoverTips();
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();
        if (_scaleOnHover)
        {
            _hoverTween?.Kill();
            _hoverTween = CreateTween();
            _hoverTween.TweenProperty((GodotObject)this, (NodePath)"scale", (Variant)SmallScale, 0.5)
                .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
        }

        if (!_showHoverTips)
            return;
        ClearHoverTips();
    }

    public void SetCardScale(Vector2 scale)
    {
        _originalScale = scale;
        Scale = _originalScale;
    }

    protected override void CreateHoverTips()
    {
        if (CardNode == null)
            return;
        NHoverTipSet.CreateAndShow((Control)this, CardNode.Model.HoverTips)
            ?.SetAlignmentForCardHolder((NCardHolder)this);
    }

    public override void _ExitTree()
    {
        if (CardNode == null)
            return;
        if (IsAncestorOf((Godot.Node)CardNode))
            CardNode.QueueFreeSafely();
        CardNode = (NCard)null;
    }
    
    public void UpdateCard()
    {
        if (!IsNodeReady() || CardNode == null)
            return;
        CardNode.UpdateVisuals(CounterPile.Counter, CardPreviewMode.Normal);
    }
}