using Godot;
using System;

public partial class Label : Godot.Label
{
	private Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Player>(new NodePath("/root/Node2D/Player"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Text = player.currentTile.Name;
	}
}
