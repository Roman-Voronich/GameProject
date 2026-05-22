using Godot;
using System;

public partial class Game : Node
{
	private static Node game;
	public static void Lose()
	{
		game.GetTree().ChangeSceneToFile("res://Scenes/Lose.tscn");
	}
	public override void _Ready()
	{
		game = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
