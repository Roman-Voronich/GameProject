using Godot;
using System;

[Tool]
public partial class StructureMap : CustomMapLayer
{
	// Called when the node enters the scene tree for the first time.
	public bool[,] buildingZone;
	private Vector4I[,] structureMap;

	public override void _Ready()
	{
	}

	public void GenerateMap(TerrianMap terrian, ResourceMap resource, int widthMap, int heightMap)
	{
		buildingZone = new bool[widthMap, heightMap];
		structureMap = new Vector4I[widthMap, heightMap];
		for (var x = 0; x < widthMap; x++)
			for (var y = 0; y < heightMap; y++)
			{
				var position = new Vector2I(x, y);
				buildingZone[x, y] = terrian.GetCellAtlasCoords(position) != new Vector2I(1, 1)
					&& resource.GetCellAtlasCoords(position).X == -1;
			}
	}

	public bool CanBuild(Structure structure, Vector2I startPosition)
	{
		var (x, y) = startPosition;
		if (x < 0 || x >= buildingZone.GetLength(0)
			|| y < 0 || y > buildingZone.GetLength(1)) return false;
		for (var i = 0; i < structure.Width; i++)
			for (var j = 0; j < structure.Height; j++)
			{
				x = startPosition.X + i;
				y = startPosition.Y + j;
				if (!buildingZone[x, y]) return false;
			}
		return true;
	}

	public void BuildStructure(int atlasX, int atlasY, int width, int height, int startX, int startY)
	{
		var info = new Vector4I(startX, startY, width, height);
		for (var i = 0; i < width; i++)
			for (var j = 0; j < height; j++)
			{
				var x = startX + i;
				var y = startY + j;
				var atlasPos = new Vector2I(atlasX + i, atlasY + j);
				structureMap[x, y] = info;
				buildingZone[x, y] = false;
				SetCell(new Vector2I(x, y), 0, atlasPos);
			}
	}

	public void BuildStructure(Structure structure, Vector2I startPosition) => 
		BuildStructure(structure.AtlasX, structure.AtlasY, structure.Width, structure.Height, startPosition.X, startPosition.Y);

	public void DestroyStructure(int startX, int startY, int width, int height)
	{
		for (var i = 0; i < width; i++)
			for (var j = 0; j < height; j++)
			{
				var x = startX + i;
				var y = startY + j;
				structureMap[x, y] = Vector4I.Zero;
				buildingZone[x, y] = true;
				EraseCell(new Vector2I(x, y));
			}
	}

	public void DestroyStructure(int x, int y)
	{
		var info = structureMap[x, y];
		if (info.Z == 0) return;
		DestroyStructure(info.X, info.Y, info.Z, info.W);
	}

	public Vector4I GetStructureInfo(Vector2I position)
	{
		var (x, y) = position;
		if (x < 0 || x >= buildingZone.GetLength(0)
			|| y < 0 || y > buildingZone.GetLength(1)) return Vector4I.Zero;
		return structureMap[x, y];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
