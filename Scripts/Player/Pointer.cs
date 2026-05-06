using Godot;
using System;

public partial class Pointer : Panel
{
	// Called when the node enters the scene tree for the first time.
	private TileMapLayer structureMap;
	private Resource resourceMap;
	private Vector2 startPos; 
	public override void _Ready()
	{
		structureMap = GetNode<TileMapLayer>(new NodePath("../Structure"));
		resourceMap = GetNode<Resource>(new NodePath("../Resource"));
		startPos = Position;
	}

	public void ChangeColor(bool isBuildMode, bool isRemoveMode)
	{
		var styleBox = GetThemeStylebox("panel") as StyleBoxFlat;
		if (!isBuildMode) styleBox.BorderColor = new Color(0xFFFFFFFF);
		else if (isRemoveMode) styleBox.BorderColor = new Color(0xFF0000FF);
		else styleBox.BorderColor = new Color(0xFFFF00FF);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = startPos + resourceMap.GetTilePos() * 32;
	}
}
