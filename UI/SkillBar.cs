using Godot;
using System;
public partial class SkillBar : Node2D
{
	[Signal]
	public delegate void SkillPressedEventHandler(SkillData skill);
	
	[Export] public HBoxContainer Container { get; set; }
	[Export] public SkillData[] Skills { get; set; } = new SkillData[0];

	private PackedScene _slotScene = GD.Load<PackedScene>("res://UI/SkillSlot.tscn");
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
