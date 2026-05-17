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
		structureMap.GenerateMap(terrianMap, resourceMap, width, height);
		walkableMap.GenerateMap(terrianMap, resourceMap, width, height);
	}

	public void DigResource(Player player, int damage)
	{
		var pos = resourceMap.GetTilePos();
		if (resourceMap.DigResource(player, damage, pos))
		{
			walkableMap.SetCell(pos, 0, new Vector2I(0, 0));
			structureMap.buildingZone[pos.X, pos.Y] = true;
		}
	}

	public void TryBuildStructure(Structure structure, Vector2I startPosition)
	{
		if (structureMap.CanBuild(structure, startPosition))
		{
			structureMap.BuildStructure(structure, startPosition);
			for (var w = 0; w < structure.Width; w++)
				for (var h = 0; h < structure.Height; h++)
					walkableMap.EraseCell(startPosition + new Vector2I(w, h));
		}
	}

	public void DestroyStructure(Vector2I pos, int width, int height)
	{
		structureMap.DestroyStructure(pos.X, pos.Y, width, height);
		for (var w = 0; w < width; w++)
				for (var h = 0; h < width; h++)
					walkableMap.SetCell(pos + new Vector2I(w, h), 0, Vector2I.Zero);
	}

	public bool CanBuild(Structure structure, Vector2I position) =>
		structureMap.CanBuild(structure, position);

	public Vector2I GetTilePos() =>
		resourceMap.GetTilePos();

	public Vector2I GetTilePos(Vector2 shift) =>
		GlobalToMap(GetGlobalMousePosition() + shift);

	public Vector2I GetStructurePos(Structure structure) =>
		GetTilePos((structure.Size - Vector2I.Zero) * -16);

	public Vector2I GlobalToMap(Vector2 gp) => 
		structureMap.LocalToMap(structureMap.ToLocal(gp));

	public Vector4I GetStructureInfo(Vector2I pos) =>
		structureMap.GetStructureInfo(pos);
}
