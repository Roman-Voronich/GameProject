using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class Map : Node2D
{
	[Export]
	public int width = 128;
	[Export]
	public int height = 128;
	[Export]
	public FastNoiseLite noise;
	[Export]
	public int seed = 67;
	private TerrianMap terrianMap;
	private ResourceMap resourceMap;
	private WalkableMap walkableMap;
	private StructureMap structureMap;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		walkableMap = GetNode<WalkableMap>(new NodePath("WalkableMap"));
		terrianMap = GetNode<TerrianMap>(new NodePath("TerrianMap"));
		resourceMap = GetNode<ResourceMap>(new NodePath("ResourceMap"));
		structureMap = GetNode<StructureMap>(new NodePath("StructureMap"));
		
		terrianMap.GenerateMap(width, height, noise, seed);
		resourceMap.GenerateMap(width, height, noise, seed);
		walkableMap.GenerateMap(terrianMap, resourceMap, width, height);
	}

	public void DigResource(Player player, int damage)
	{
		var pos = resourceMap.GetTilePos();
		if (resourceMap.DigResource(player, damage, pos))
		{
			walkableMap.SetCell(pos, 0, new Vector2I(0, 0));
		}
	}

	public Vector2I GetTilePos() =>
		resourceMap.GetTilePos();

	public Vector2I GetTilePos(Vector2 shift) =>
		GlobalToMap(GetGlobalMousePosition() + shift);

	public Vector2I GlobalToMap(Vector2 gp) => 
		structureMap.LocalToMap(structureMap.ToLocal(gp));
}
