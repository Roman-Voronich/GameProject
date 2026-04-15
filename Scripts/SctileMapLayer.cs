using Godot;
using System;

public partial class SctileMapLayer : TileMapLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// for (var x = -20; x < 40; x++)
		// 	for (var y = -20; y < 30; y++)
		// 		SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
