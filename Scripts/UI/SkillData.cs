using Godot;
using System;
[GlobalClass]
public partial class SkillData : Resource
{
    [Export] public string Id { get; set; } = string.Empty;
    [Export] public string Name { get; set; } = string.Empty;
    [Export] public Texture2D Icon { get; set; }
    [Export] public string Keybind { get; set; } = "1";
    [Export] public float Cooldown { get; set; } = 0f;
}
