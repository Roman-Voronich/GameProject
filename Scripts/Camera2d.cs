using Godot;
using System;

public partial class Camera2d : Camera2D
{
	private float cameraZoom = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_mouse_scroll_down"))
			cameraZoom = Math.Max(cameraZoom * 0.8f, 0.5f);
		if (Input.IsActionJustPressed("ui_mouse_scroll_up"))
			cameraZoom = Math.Min(cameraZoom * 1.25f, 2f);
		Zoom += Vector2.One * (cameraZoom - Zoom.X) * 0.2f;
	}
}
