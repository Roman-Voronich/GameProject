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
	private static TerrianMap terrianMap;
	private static ResourceMap resourceMap;
	private static  WalkableMap walkableMap;
	private static StructureMap structureMap;
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

	public static void DigResource(Player player, int damage)
	{
		var pos = resourceMap.GetTilePos();
		if (resourceMap.DigResource(player, damage, pos))
		{
			walkableMap.SetCell(pos, 0, new Vector2I(0, 0));
		}
	}
	
	public static bool CanPlaceBuilding(Vector2I startPos, Vector2I size)
	{
		for (int x = 0; x < size.X; x++)
		{
			for (int y = 0; y < size.Y; y++)
			{
				Vector2I tile = startPos + new Vector2I(x, y);

				if (structureMap.GetCellSourceId(tile) != -1)
					return false;

				if (IsWater(terrianMap, tile))
					return false;

				if (resourceMap.GetCellSourceId(tile) != -1)
					return false;
			}
		}
		return true;
	}
	private static bool IsWater(TileMapLayer terrainMap, Vector2I tile)
	 => terrainMap.GetCellAtlasCoords(tile) == new Vector2I(1, 1);
	

	public static Vector2I GetTilePos() =>
		resourceMap.GetTilePos();

	public static Vector2I GetTilePos(Vector2 shift) =>
		GlobalToMap(terrianMap.GetGlobalMousePosition() + shift);

	public static Vector2I GlobalToMap(Vector2 gp) => 
		structureMap.LocalToMap(structureMap.ToLocal(gp));
	
}
