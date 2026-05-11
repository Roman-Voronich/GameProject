using Godot;
using System;

[Tool]
public partial class Terrian : MapFromNoise
{
	// Called when the node enters the scene tree for the first time.

    public override void GenerateMap()
    {
        Clear();
        noise.Seed = seed;
        var sourceId = TileSet.GetSourceId(0);
        for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var value = (noise.GetNoise2D(x, y) + 1) / 2;
                var atlasCoords = value > 0.75f ? new Vector2I(0, 0)
								: value > 0.4f ? new Vector2I(1, 0)
								: value > 0.37f ? new Vector2I(0, 1)
								: new Vector2I(1, 1);
                SetCell(new Vector2I(x, y), sourceId, atlasCoords);
            }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
