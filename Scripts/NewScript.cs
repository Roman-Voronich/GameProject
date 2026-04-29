using Godot;
using System;

public class TileInfo
{
    public int atlasId;
    public Vector2I atlasCoord;
    public string Name;

    public TileInfo(string name, int atlasId, Vector2I atlasCoord)
    {
        Name = name;
        this.atlasId = atlasId;
        this.atlasCoord = atlasCoord;
    }
}
