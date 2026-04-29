using Godot;
using System;

[GlobalClass]
public partial class Player : Node2D
{
	[Export]
	private int Speed = 10;
	private MapFromNoise _tileMap;
	private Camera2D Camera;
	private int _tileIndex = 0;
	public TileInfo currentTile;
	private Panel tilePointer;

	private bool isBuildMode = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tileMap = GetNode<MapFromNoise>(new NodePath("../Map"));
		tilePointer = GetNode<Panel>(new NodePath("../tilePointer"));
		Camera = GetNode<Camera2D>(new NodePath("Camera2D"));
		tilePointer.Visible = false;
		currentTile = _tileMap.tiles[_tileIndex];
	}

    public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_change_mode"))
			changeMode();
	}

	private void changeMap()
	{
		var mouseGlobalPos = GetGlobalMousePosition();
		var tilePos = _tileMap.LocalToMap(_tileMap.ToLocal(mouseGlobalPos));
		_tileMap.SetCell(tilePos, currentTile.atlasId, currentTile.atlasCoord);
	}

	private void changeMode()
	{
		isBuildMode = !isBuildMode;
		tilePointer.Visible = isBuildMode;
	}

	private void changeTile()
	{
		_tileIndex = (_tileIndex + 1) % _tileMap.tiles.Length;
		currentTile = _tileMap.tiles[_tileIndex];
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		DoMove();
		DoBuild();
	}

	private void DoBuild()
	{
		if (isBuildMode
			&& Input.IsActionPressed("ui_left_click"))
			changeMap();
		if (Input.IsActionJustPressed("ui_change_tile"))
			changeTile();
	}

	private void DoMove()
	{
		var direction = new Vector2();
		direction.X = Input.GetAxis("ui_left", "ui_right");
		direction.Y = Input.GetAxis("ui_up", "ui_down");
		direction.Normalized();
		Position += direction * Speed / Camera.Zoom;
	}
}