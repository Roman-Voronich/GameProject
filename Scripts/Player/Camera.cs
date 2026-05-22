using Godot;
using System;

public partial class Camera : Camera2D
{
	[Export] public int Speed;
	private float cameraZoom = 1;
	private Vector2 size;
	private Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		size = GetViewport().GetVisibleRect().Size;
		GD.Print("Camera size : ", size);
		player = GetNode<Player>(new NodePath("%Player"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		DoZoom();
		DoMove();
	}

	
	private void DoZoom()
	{
		if (Input.IsActionJustPressed("ui_mouse_scroll_down"))
			cameraZoom = Math.Max(cameraZoom * 0.8f, 0.5f);
		if (Input.IsActionJustPressed("ui_mouse_scroll_up"))
			cameraZoom = Math.Min(cameraZoom * 1.25f, 2f);
		Zoom += Vector2.One * (cameraZoom - Zoom.X) * 0.2f;
	}

	private void DoMove()
	{
		var newPosition = player.Position + Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down") * Speed / Zoom;
		newPosition.X = DoBound(newPosition.X, 0, 4096);
		newPosition.Y = DoBound(newPosition.Y, 0, 4096);
		player.Position = newPosition;
	}

	private float DoBound(float x, float left, float right)
	{
		if (x < left) return left;
		if (x > right) return right;
		return x;
	}
}
