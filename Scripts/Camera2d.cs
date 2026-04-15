using Godot;
using System;

public partial class Camera2d : Camera2D
{
	private int Speed = 10;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			switch (keyEvent.Keycode) {
				case Key.A:
					Position += new Vector2(-Speed, 0);
					break;
				case Key.D:
					Position += new Vector2(Speed, 0);
					break;
				case Key.S:
					Position += new Vector2(0, Speed);
					break;
				case Key.W:
					Position += new Vector2(0, -Speed);
					break;
			}
		}
    }


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
