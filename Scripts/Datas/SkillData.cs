using Godot;
using System;
[GlobalClass]
public partial class SkillData : Resource
{
    [Export] public string Name { get; set; } = string.Empty;
    [Export] public Texture2D Icon { get; set; }
    [Export] public string Keybind { get; set; } = "1";
    [Export] public float Cooldown { get; set; } = 0f;
    [Export] public bool IsTargetable { get; set; } = false;
}
