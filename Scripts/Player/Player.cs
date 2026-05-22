using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Node2D
{
	private Vector2I currentTile = new(0, 0);
	private Structure currentStructure = new(2, 2, 1, 0);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		map = GetNode<Map>(new NodePath("../Map"));
		ModeChanged += x => GetNode<Pointer>(new NodePath("../Map/Pointer")).ChangeMode(x, currentStructure);
	}

    public override void _Input(InputEvent @event)
	{
		if (Mode == PlayerMode.Build
			&& IsKeyJustPressed(@event, Key.F))
			isManyChange = !isManyChange;
		if (Mode == PlayerMode.Nothing
			&& @event.IsActionPressed("ui_left_click"))
			map.DigResource(this, 1);
	}

	private bool IsKeyJustPressed(InputEvent e, Key key) =>
		e is InputEventKey inputKey
		&& inputKey.Keycode == key
		&& inputKey.Pressed;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Mode == PlayerMode.Build
		|| Mode == PlayerMode.Destroy) DoBuild();
	}
}