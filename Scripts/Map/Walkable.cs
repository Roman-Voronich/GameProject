using Godot;
using System;

public partial class Walkable : TileMapLayer
{
	[Export]
	public int width = 128;
	[Export]
	public int height = 128;
	private TileMapLayer terrian;
	private TileMapLayer resource;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		terrian = GetNode<TileMapLayer>(new NodePath("../Terrian"));
		resource = GetNode<TileMapLayer>(new NodePath("../Resource"));
		GenerateMap();
	}

	private void GenerateMap()
	{
		for (var x = 0; x < width; x++)
			for (var y = 0; y < height; y++)
			{
				var position = new Vector2I(x, y);
				if (terrian.GetCellAtlasCoords(position) != new Vector2I(1, 1)
					&& resource.GetCellAtlasCoords(position).X == -1)
					SetCell(position, 0, Vector2I.Zero);
			}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
