using Godot;
using System;

[GlobalClass]
public partial class SkillData : Resource
{

	[Export]
	public string Name { get; set; }
	[Export]
	public Texture2D Icon { get; set; }
	[Export]
	public float CooldownSeconds { get; set; }
	
}
