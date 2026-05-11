using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Node2D
{
	[Export]
	private int Speed = 16;
	private Camera2D camera;
	private float cameraZoom = 1;
	private Vector2I currentTile = new(0, 0);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		resourceMap = GetNode<Resource>(new NodePath("../Map/Resource"));
		structureMap = GetNode<TileMapLayer>(new NodePath("../Map/Structure"));
		terrianMap = GetNode<Terrian>(new NodePath("../Map/Terrian"));
		camera = GetNode<Camera2D>(new NodePath("Camera"));
		inventory = new Dictionary<string, int>();
        pointer = GetNode<Pointer>(new NodePath("../Map/Pointer"));
		walkableMap = GetNode<TileMapLayer>(new NodePath("../Map/Walkable"));
	}

    public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_change_mode"))
			ChangeMode();
		if (isBuildMode
			&& IsKeyJustPressed(@event, Key.E))
			isRemoveMode = !isRemoveMode;
		if (isBuildMode
			&& IsKeyJustPressed(@event, Key.F))
			isManyChange = !isManyChange;
		if (!isBuildMode
			&& @event.IsActionPressed("ui_left_click"))
			resourceMap.DigResource(inventory, 1);
	}

	private bool IsKeyJustPressed(InputEvent e, Key key) =>
		e is InputEventKey inputKey
		&& inputKey.Keycode == key
		&& inputKey.Pressed;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		DoMove();
		DoZoom();
		if (isBuildMode) DoBuild();
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