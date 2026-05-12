using Godot;
using System;

[Tool]
public partial class WalkableMap : TileMapLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void GenerateMap(TileMapLayer terrian, TileMapLayer resource, int width, int height)
	{
		for (var x = 0; x < width; x++)
			for (var y = 0; y < height; y++)
			{
				var position = new Vector2I(x, y);
				if (terrian.GetCellAtlasCoords(position) != new Vector2I(1, 1)
					&& resource.GetCellAtlasCoords(position).X == -1)
					SetCell(position, 0, Vector2I.Zero);
				else EraseCell(position);
			}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
