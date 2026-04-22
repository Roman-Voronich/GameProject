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
		if (isBuildMode
			&& @event is InputEventMouseButton mouse
			&& mouse.Pressed)
			changeMap(mouse);
	}

	private void changeMap(InputEventMouseButton mouse)
	{
		var mouseGlobalPos = camera.GetGlobalMousePosition();
		var tilePos = _tileMap.LocalToMap(_tileMap.ToLocal(mouseGlobalPos));
		if (mouse.ButtonIndex == MouseButton.Left)
			_tileMap.SetCell(tilePos, 0, new Vector2I(1, 0));
		else if (mouse.ButtonIndex == MouseButton.Right)
			_tileMap.SetCell(tilePos, 0, new Vector2I(1, 1));
	}

	private void changeMode()
	{
		isBuildMode = !isBuildMode;
		GD.Print(isBuildMode, camera.Scale);
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var direction = new Vector2();
		direction.X = Input.GetAxis("ui_left", "ui_right");
		direction.Y = Input.GetAxis("ui_up", "ui_down");
		if (Input.IsActionJustPressed("ui_mouse_scroll_down"))
			cameraZoom = Math.Max(cameraZoom * 0.8f, 0.5f);
		if (Input.IsActionJustPressed("ui_mouse_scroll_up"))
			cameraZoom = Math.Min(cameraZoom * 1.25f, 2f);
		
		camera.Zoom += new Vector2(1, 1) * (cameraZoom - camera.Zoom.X) * 0.2f;
		direction.Normalized();

		Position += direction * Speed / camera.Zoom;
	}
}