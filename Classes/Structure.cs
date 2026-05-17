using System;
using Godot;

public class Structure
{
    public Vector2I Size { get; }
    public int Width { get => Size.X; }
    public int Height { get => Size.Y; }
    public Vector2I AtlasPos { get; }
    public int AtlasX { get => AtlasPos.X; }
    public int AtlasY { get => AtlasPos.Y; }
    private Action<Map, Vector2I, Structure> _buildLogic = (m, p, s) =>
    {
        m.TryBuildStructure(s, p);
    };
    private Func<Map, Vector2I, Structure, bool> _placementLogic = (m, p, s) =>
    {
        return true;
    };

    public Structure(int width, int height, int x, int y)
    {
        Size = new Vector2I(width, height);
        AtlasPos = new Vector2I(x, y);
    }

    public Structure(Vector2I size, Vector2I atlasPos)
    {
        Size = size;
        AtlasPos = atlasPos;
    }

    public bool CanBuild(Map map, Vector2I startPos) => _placementLogic(map, startPos, this);
    public void DoBuild(Map map, Vector2I startPos) => _buildLogic(map, startPos, this);

    public static readonly Structure test = new(1, 1, 0, 0);
}