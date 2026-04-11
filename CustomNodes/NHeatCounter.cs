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
using MegaCrit.Sts2.Core.Nodes.Vfx;
using StS2CharTest;
using StS2CharTest.Cards;
using StS2CharTest.Code.Character;
using StS2CharTest.CustomNodes;

public partial class NHeatCounter : Control
{
  private static readonly StringName _h = new StringName("h");
	private static readonly StringName _v = new StringName("v");
  private static readonly StringName _s = new StringName("s");
  private Player? _player;
  private MegaRichTextLabel _label;
  private Godot.Control _rotationLayers;
  private Godot.Control _icon;
  private ShaderMaterial _hsv;
  private float _velocity;
  private int _displayedHeatCount;
  private Tween? _vTween;
  private Tween? _hTween;
  private bool _isListeningToCombatState;
  private HoverTip _hoverTip;

  private CharTestParticlesContainer particlesContainer;

  public override void _Ready()
  {
    particlesContainer = GetNode<CharTestParticlesContainer>("%HeatCounterVfx");
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
    MainFile.Logger.Info("Heat Changed. Old heat: " + oldHeat + ", New heat: " + newHeat);
  }

  public override void _Process(double delta)
  {
    if (this._player == null)
      return;
    float num = HeatResource.Amount.Get(_player.PlayerCombatState) == 0 ? 5f : 30f;
    for (int idx = 0; idx < this._rotationLayers.GetChildCount(); ++idx)
      this._rotationLayers.GetChild<Godot.Control>(idx).RotationDegrees += (float) delta * num * (float) (idx + 1);
  }

  private void UpdateHeatCount(int oldCount, int newCount)
  {
    if (oldCount == 0)
    {
      _vTween?.Kill();
      _hsv.SetShaderParameter(_v, (Variant)1f);
    }
    this.SetHeatCountText(newCount);
    if (newCount > oldCount)
    {
      _vTween?.Kill();
      _vTween = CreateTween();
      _vTween.TweenMethod(Callable.From<float>(new Action<float>(this.UpdateShaderV)), (Variant)2f, (Variant)1f,
        0.20000000298023224);
      MainFile.Logger.Info("Playing Heat Gain VFX");
      particlesContainer.Restart();
    }
    
    _hTween?.Kill();
    _hTween = CreateTween();
    Variant currentHue = _hsv.GetShaderParameter(_h);
    _hTween.TweenMethod(Callable.From(new Action<float>(UpdateShaderH)), currentHue, (Variant)(newCount / 20f * 0.4f), 0.2f);
    

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
  
  private void UpdateShaderH(float hue)
  {
    this._hsv.SetShaderParameter(_h, (Variant) hue);

    particlesContainer.SetHueForParticles(hue);
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
      bool ShouldAlwaysShowHeatCounter = _player.Character.GetType() == typeof(CharTest);
      this.Visible = this.Visible || ShouldAlwaysShowHeatCounter || heat > 0;
    }
    MainFile.Logger.Info("Heat Counter Visibility: " + Visible);
  }
}
