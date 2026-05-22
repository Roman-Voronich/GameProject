using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class ResourceMap : CustomMapLayer
{
    private TileResourceInfo[,] resources;

    public bool DigResource(Player player, int damage, Vector2I position)
    {
        var (x, y) = position;
        if (x < 0 || x >= resources.GetLength(0)
			|| y < 0 || y > resources.GetLength(1)) return false;
        var resource = resources[x, y];
        if (resource == null) return false;
        resource.hp -= damage;
        if (resource.hp <= 0)
        {
            Inventory.ChangeCountResource(resource.type, resource.count);
            EraseCell(position);
            resources[x, y] = null;
            return true;
        }
    return false;
    }

    public void GenerateMap(int width, int height, FastNoiseLite noise, int seed)
    {
        noise.Seed = seed;
        resources = new TileResourceInfo[width, height];
        var sourceId = TileSet.GetSourceId(0);
        for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var value = (noise.GetNoise2D(x, y) + 1) / 2;
				var atlasCoords = new Vector2I(0, 0);
				if (value > 0.76f)
                {
					var rand = GD.Randf();
                    if (rand > 0.8) // железо
                    {
                        atlasCoords = new Vector2I(0, 1);
                        resources[x, y] = new TileResourceInfo(ResourceType.Iron, 2, 15);
                    }
                    else if (rand > 0.6) // медь
                    {
                        atlasCoords = new Vector2I(1, 1);
                        resources[x, y] = new TileResourceInfo(ResourceType.Copper, 3, 12);
                    }
                    else if (rand > 0.2) // камень
                    {
                        atlasCoords = new Vector2I(1, 0);
                        resources[x, y] = new TileResourceInfo(ResourceType.Stone, 4, 6);
                    }
                    else continue;
                }
				else if (value > 0.6f && value < 0.7f)
                {
					atlasCoords = new Vector2I(0, 0);
                    resources[x, y] = new TileResourceInfo(ResourceType.Wood, 5, 3);
                }
				else continue;
                SetCell(new Vector2I(x, y), sourceId, atlasCoords);
            }
        GD.Print("Resources Generated");
    }
}
