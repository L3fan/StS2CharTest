using Godot;
using System;
using System.ComponentModel;
using Godot.Bridge;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using StS2CharTest.Cards;

public partial class NHeatCounter : Control
{
	private static readonly StringName _v = new StringName("v");
  private static readonly StringName _s = new StringName("s");
  private static readonly string _heatGainVfxPath = SceneHelper.GetScenePath("vfx/heat_gain_vfx");
  private Player? _player;
  private MegaRichTextLabel _label;
  private Godot.Control _rotationLayers;
  private Godot.Control _icon;
  private ShaderMaterial _hsv;
  private float _lerpingHeatCount;
  private float _velocity;
  private int _displayedHeatCount;
  private Tween? _hsvTween;
  private bool _isListeningToCombatState;
  private HoverTip _hoverTip;

  public static IEnumerable<string> AssetPaths
  {
    get
    {
      return (IEnumerable<string>)new List<string>() { _heatGainVfxPath };
    }
  }

  public override void _Ready()
  {
    this._label = this.GetNode<MegaRichTextLabel>((NodePath) "%CountLabel");
    this._rotationLayers = this.GetNode<Godot.Control>((NodePath) "%RotationLayers");
    this._icon = this.GetNode<Godot.Control>((NodePath) "Icon");
    this._hsv = (ShaderMaterial) this._icon.Material;
    LocString description = new LocString("static_hover_tips", "HEAT_COUNT.description");
    description.Add("singleHeatIcon", "[img]res://images/sts2chartest/powers/heat_power.png[/img]");
    this._hoverTip = new HoverTip(new LocString("static_hover_tips", "HEAT_COUNT.title"), description);
    long num1 = (long) this.Connect(Godot.Control.SignalName.MouseEntered, Callable.From(new Action(this.OnHovered)));
    long num2 = (long) this.Connect(Godot.Control.SignalName.MouseExited, Callable.From(new Action(this.OnUnhovered)));
    this.Visible = false;
  }

  public override void _EnterTree()
  {
    base._EnterTree();
    this.ConnectHeatChangedSignal();
  }

  public override void _ExitTree()
  {
    base._ExitTree();
    if (this._player == null || !this._isListeningToCombatState)
      return;
    Action<int,int> heatChangedAction = HeatResource.HeatChanged.Get(_player.PlayerCombatState);
    heatChangedAction -= new Action<int, int>(this.OnHeatChanged);
    HeatResource.HeatChanged.Set(_player.PlayerCombatState, heatChangedAction);
    this._isListeningToCombatState = false;
  }

  private void ConnectHeatChangedSignal()
  {
    if (this._player == null || this._isListeningToCombatState)
      return;
    Action<int,int> heatChangedAction = HeatResource.HeatChanged.Get(_player.PlayerCombatState);
    heatChangedAction += new Action<int, int>(this.OnHeatChanged);
    HeatResource.HeatChanged.Set(_player.PlayerCombatState, heatChangedAction);
    this._isListeningToCombatState = true;
  }

  public void Initialize(Player player)
  {
    this._player = player;
    this.ConnectHeatChangedSignal();
    this.RefreshVisibility();
  }

  private void OnHovered()
  {
    NHoverTipSet.CreateAndShow((Godot.Control) this, (IHoverTip) this._hoverTip).GlobalPosition = this.GlobalPosition + new Vector2(-34f, -300f);
  }

  private void OnUnhovered() => NHoverTipSet.Remove((Godot.Control) this);

  private void OnHeatChanged(int oldHeat, int newHeat)
  {
    this.UpdateHeatCount(oldHeat, newHeat);
    this.RefreshVisibility();
  }

  public override void _Process(double delta)
  {
    if (this._player == null)
      return;
    float num = HeatResource.Amount.Get(_player.PlayerCombatState) == 0 ? 5f : 30f;
    for (int idx = 0; idx < this._rotationLayers.GetChildCount(); ++idx)
      this._rotationLayers.GetChild<Godot.Control>(idx).RotationDegrees += (float) delta * num * (float) (idx + 1);
    this._lerpingHeatCount = MathHelper.SmoothDamp(this._lerpingHeatCount, (float) HeatResource.Amount.Get(_player.PlayerCombatState), ref this._velocity, 0.1f, (float) delta);
    this.SetHeatCountText(Mathf.RoundToInt(this._lerpingHeatCount));
  }

  private void UpdateHeatCount(int oldCount, int newCount)
  {
    if (newCount < oldCount)
    {
      this._hsvTween?.Kill();
      this._hsv.SetShaderParameter(_v, (Variant) 1f);
      this._lerpingHeatCount = (float) newCount;
      this.SetHeatCountText(newCount);
    }
    else
    {
      if (newCount <= oldCount)
        return;
      this._hsvTween?.Kill();
      this._hsvTween = this.CreateTween();
      this._hsvTween.TweenMethod(Callable.From<float>(new Action<float>(this.UpdateShaderV)), (Variant) 2f, (Variant) 1f, 0.20000000298023224);
      Node2D node2D = PreloadManager.Cache.GetAsset<PackedScene>(_heatGainVfxPath).Instantiate<Node2D>();
      this.AddChildSafely((Node) node2D);
      this.MoveChild((Node) node2D, 0);
      node2D.Position = this.Size / 2f;
    }
  }

  private void SetHeatCountText(int heat)
  {
    if (this._displayedHeatCount == heat)
      return;
    this._displayedHeatCount = heat;
    this._label.AddThemeColorOverride(ThemeConstants.Label.FontColor, heat == 0 ? StsColors.red : StsColors.cream);
    this._label.Text = $"[center]{heat}[/center]";
    if (heat == 0)
    {
      this._hsv.SetShaderParameter(_s, (Variant) 0.5f);
      this._hsv.SetShaderParameter(_v, (Variant) 0.85f);
    }
    else
    {
      this._hsv.SetShaderParameter(_s, (Variant) 1f);
      this._hsv.SetShaderParameter(_v, (Variant) 1f);
    }
  }

  private void UpdateShaderV(float value)
  {
    this._hsv.SetShaderParameter(_v, (Variant) value);
  }

  private void RefreshVisibility()
  {
    if (this._player == null)
    {
      this.Visible = false;
    }
    else
    {
      int heat = (int)HeatResource.Amount.Get(_player.PlayerCombatState);
      bool ShouldAlwaysShowHeatCounter = _player.Character.GetType() == typeof(CharTestModel);
      this.Visible = this.Visible || ShouldAlwaysShowHeatCounter || heat > 0;
    }
  }
}
