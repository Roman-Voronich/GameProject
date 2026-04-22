using Godot;
using System;

public partial class Player : Node2D
{
	[Export]
	private int Speed = 10;
	private MapFromNoise _tileMap;
	private Camera2D camera;
	private Vector2I cameraSize;
	private float cameraZoom = 1;
	private Vector2I currentTile = new(0, 0);

	private bool isBuildMode = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tileMap = GetNode<MapFromNoise>(new NodePath("../Map"));
		camera = GetNode<Camera2D>(new NodePath("Camera2D"));
		cameraSize = GetTree().Root.Size;
	}

    public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_change_mode"))
			changeMode();
	}

	private void changeMap()
	{
		var mouseGlobalPos = camera.GetGlobalMousePosition();
		var tilePos = _tileMap.LocalToMap(_tileMap.ToLocal(mouseGlobalPos));
		_tileMap.SetCell(tilePos, 0, currentTile);
	}

	private void changeMode()
	{
		isBuildMode = !isBuildMode;
	}

	private void changeTile()
	{
		var x = currentTile.X;
		var y = currentTile.Y;
		currentTile = new Vector2I(x + 1, y + (x + 1) % 2) % 2;
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		DoMove();
		DoZoom();
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

	private void DoZoom()
	{
		if (Input.IsActionJustPressed("ui_mouse_scroll_down"))
			cameraZoom = Math.Max(cameraZoom * 0.8f, 0.5f);
		if (Input.IsActionJustPressed("ui_mouse_scroll_up"))
			cameraZoom = Math.Min(cameraZoom * 1.25f, 2f);
		camera.Zoom += Vector2.One * (cameraZoom - camera.Zoom.X) * 0.2f;
	}

	private void DoMove()
	{
		var direction = new Vector2();
		direction.X = Input.GetAxis("ui_left", "ui_right");
		direction.Y = Input.GetAxis("ui_up", "ui_down");
		direction.Normalized();
		Position += direction * Speed / camera.Zoom;
	}
}