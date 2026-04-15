using Godot;
using System;

public partial class Player : Node2D
{
	private int Speed = 10;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    public override void _Input(InputEvent @event)
    {
    }


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var direction = new Vector2();
		direction.X = Input.GetAxis("ui_left", "ui_right");
		direction.Y = Input.GetAxis("ui_up", "ui_down");

		direction.Normalized();

		Position += direction * Speed;
	}
}
