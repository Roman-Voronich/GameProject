using Godot;
using System;

public partial class SkillSlot : Button
{
	[Signal]
	public delegate void SkillClickedEventHandler(SkillData skill, bool isToggledOn);
	
	public SkillData Data { get; private set; }
	private TextureRect _icon;
	private Label _keybindLabel;
	private ColorRect _cooldownOverlay;

	public override void _Ready()
	{
		_icon = GetNode<TextureRect>("Icon");
		_keybindLabel = GetNode<Label>("Keybind");
		_cooldownOverlay = GetNode<ColorRect>("CooldownOverlay");
		_cooldownOverlay.Visible = false;
		
		ToggleMode = true;
		Toggled += OnToggled;
	}
	
	public void Setup(SkillData data)
	{
		Data = data;
		_icon.Texture = data.Icon;
		_keybindLabel.Text = Data.Keybind;
	}

	private void OnToggled(bool toggledOn)
	{
		EmitSignal(SignalName.SkillClicked, Data, toggledOn);
	}

	
	public override void _Process(double delta)
	{
	}
}
