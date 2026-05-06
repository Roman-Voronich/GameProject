using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class Resource : MapFromNoise
{
    private TileResourceInfo[,] resources;

    public override void _Ready()
    {
        GenerateMap();
    }

    public void DigResource(Dictionary<string, int> inventory, int damage)
    {
        var (x, y) = GetTilePos();
        try
        {
            var resource = resources[x, y];
            if (resource == null) return;
            resource.hp -= damage;
            if (resource.hp <= 0)
            {
                if (!inventory.TryAdd(resource.type, resource.count))
                    inventory[resource.type] += resource.count;
                GD.Print("You dig resource");
                GD.Print("You have ", inventory[resource.type], ' ', resource.type);
                EraseCell(GetTilePos());
                resources[x, y] = null;
            }
        }
        catch (Exception e)
        {
            GD.PrintErr("OutBounds");
        }
    }

    public override void GenerateMap()
    {
        Clear();
        noise.Seed = seed;
        resources = new TileResourceInfo[width, height];
        var sourceId = TileSet.GetSourceId(0);
        for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var value = (noise.GetNoise2D(x, y) + 1) / 2;
				var atlasCoords = new Vector2I(0, 0);
				if (value > 0.78f)
                {
					atlasCoords = new Vector2I(1, 0);
                    resources[x, y] = new TileResourceInfo("stone", 2, 12);
                }
				else if (value > 0.6f && value < 0.7f)
                {
					atlasCoords = new Vector2I(0, 0);
                    resources[x, y] = new TileResourceInfo("wood", 5, 3);
                }
				else continue;
                SetCell(new Vector2I(x, y), sourceId, atlasCoords);
            }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
