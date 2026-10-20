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

public partial class NRageCounter : Control
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
  private int _displayedRageCount;
  private Tween? _vTween;
  private Tween? _hTween;
  private bool _isListeningToCombatState;
  private HoverTip _hoverTip;

  private CharTestParticlesContainer particlesContainer;

  public override void _Ready()
  {
    particlesContainer = GetNode<CharTestParticlesContainer>("%RageCounterVfx");
    this._label = this.GetNode<MegaRichTextLabel>((NodePath) "%CountLabel");
    this._rotationLayers = this.GetNode<Godot.Control>((NodePath) "%RotationLayers");
    this._icon = this.GetNode<Godot.Control>((NodePath) "Icon");
    this._hsv = (ShaderMaterial) this._icon.Material;
    LocString description = new LocString("static_hover_tips", "HEAT_COUNT.description");
    description.Add("singleRageIcon", "[img]res://images/sts2chartest/powers/rage_power.png[/img]");
    this._hoverTip = new HoverTip(new LocString("static_hover_tips", "HEAT_COUNT.title"), description);
    long num1 = (long) this.Connect(Godot.Control.SignalName.MouseEntered, Callable.From(new Action(this.OnHovered)));
    long num2 = (long) this.Connect(Godot.Control.SignalName.MouseExited, Callable.From(new Action(this.OnUnhovered)));
    this.Visible = false;
  }

  public override void _EnterTree()
  {
    base._EnterTree();
    this.ConnectRageChangedSignal();
  }

  public override void _ExitTree()
  {
    base._ExitTree();
    if (this._player == null || !this._isListeningToCombatState)
      return;
    Action<int,int> rageChangedAction = RageResource.RageChanged.Get(_player.PlayerCombatState);
    rageChangedAction -= new Action<int, int>(this.OnRageChanged);
    RageResource.RageChanged.Set(_player.PlayerCombatState, rageChangedAction);
    this._isListeningToCombatState = false;
  }

  private void ConnectRageChangedSignal()
  {
    if (this._player == null || this._isListeningToCombatState)
      return;
    Action<int,int> rageChangedAction = RageResource.RageChanged.Get(_player.PlayerCombatState);
    rageChangedAction += new Action<int, int>(this.OnRageChanged);
    RageResource.RageChanged.Set(_player.PlayerCombatState, rageChangedAction);
    this._isListeningToCombatState = true;
  }

  public void Initialize(Player player)
  {
    this._player = player;
    this.ConnectRageChangedSignal();
    this.RefreshVisibility();
  }

  private void OnHovered()
  {
    NHoverTipSet.CreateAndShow((Godot.Control) this, (IHoverTip) this._hoverTip).GlobalPosition = this.GlobalPosition + new Vector2(-34f, -300f);
  }

  private void OnUnhovered() => NHoverTipSet.Remove((Godot.Control) this);

  private void OnRageChanged(int oldRage, int newRage)
  {
    this.UpdateRageCount(oldRage, newRage);
    this.RefreshVisibility();
    MainFile.Logger.Info("Rage Changed. Old rage: " + oldRage + ", New rage: " + newRage);
  }

  public override void _Process(double delta)
  {
    if (this._player == null)
      return;
    float num = RageResource.Amount.Get(_player.PlayerCombatState) == 0 ? 5f : 30f;
    for (int idx = 0; idx < this._rotationLayers.GetChildCount(); ++idx)
      this._rotationLayers.GetChild<Godot.Control>(idx).RotationDegrees += (float) delta * num * (float) (idx + 1);
  }

  private void UpdateRageCount(int oldCount, int newCount)
  {
    if (oldCount == 0)
    {
      _vTween?.Kill();
      _hsv.SetShaderParameter(_v, (Variant)1f);
    }
    this.SetRageCountText(newCount);
    if (newCount > oldCount)
    {
      _vTween?.Kill();
      _vTween = CreateTween();
      _vTween.TweenMethod(Callable.From<float>(new Action<float>(this.UpdateShaderV)), (Variant)2f, (Variant)1f,
        0.20000000298023224);
      MainFile.Logger.Info("Playing Rage Gain VFX");
      particlesContainer.Restart();
    }
    
    _hTween?.Kill();
    _hTween = CreateTween();
    Variant currentHue = _hsv.GetShaderParameter(_h);
    _hTween.TweenMethod(Callable.From(new Action<float>(UpdateShaderH)), currentHue, (Variant)(newCount / 20f * 0.4f), 0.2f);
    

  }

  private void SetRageCountText(int rage)
  {
    if (this._displayedRageCount == rage)
      return;
    this._displayedRageCount = rage;
    this._label.AddThemeColorOverride(ThemeConstants.Label.FontColor, rage == 0 ? StsColors.red : StsColors.cream);
    this._label.Text = $"[center]{rage}[/center]";
    if (rage == 0)
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
      int rage = (int)RageResource.Amount.Get(_player.PlayerCombatState);
      bool ShouldAlwaysShowRageCounter = _player.Character.GetType() == typeof(CharTest);
      this.Visible = this.Visible || ShouldAlwaysShowRageCounter || rage > 0;
    }
    MainFile.Logger.Info("Rage Counter Visibility: " + Visible);
  }
}
