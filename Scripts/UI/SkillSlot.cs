using Godot;
using System;
public partial class SkillSlot : Button
{
	
	[Signal]
	public delegate void SkillPressedEventHandler(SkillData skill);
	private TextureRect _iconRect;
	private Label _cooldownLabel;
	private SkillData _data;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_iconRect = GetNode<TextureRect>("Icon");
		_cooldownLabel = GetNode<Label>("CooldownLabel");
		Pressed += OnSlotPressed;
	}
	public void Setup(SkillData data)
	{
		if (data == null) return;
		_data = data;
		_iconRect.Texture = data.Icon;
		_cooldownLabel.Text = $"{data.CooldownSeconds:F1}s";
	}
	private void OnSlotPressed()
	{
		if (_data != null)
		{
			EmitSignal(SignalName.SkillPressed, _data);
		}
	}
}
