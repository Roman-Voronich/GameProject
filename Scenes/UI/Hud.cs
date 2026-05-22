using Godot;
using System;

public partial class Hud : Control
{
	public override void _Ready()
	{
		foreach (var child in GetChildren())
		{
			child._Ready();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
