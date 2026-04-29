using Godot;
using System;

public partial class TilePointer : Panel
{
	private MapFromNoise _tileMap;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tileMap = GetNode<MapFromNoise>(new NodePath("../Map"));
		Size = new Vector2(32, 32);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var mousePosition = GetGlobalMousePosition();
		var tilePosition = _tileMap.LocalToMap(_tileMap.ToLocal(mousePosition));
		Position = tilePosition * 32;
	}
}
