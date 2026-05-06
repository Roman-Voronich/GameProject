using Godot;
using System;

[Tool, GlobalClass]
public partial class MapFromNoise : TileMapLayer
{
    private FastNoiseLite _noise;
    private int _width = 128;
    private int _height = 128;
    private int _seed = 10;

    [Export]
    public FastNoiseLite noise
    {
        get => _noise;
        set
        {
            _noise = value;
            if (Engine.IsEditorHint())
                GenerateMap();
        }
    }

    [Export]
    public int width
    {
        get => _width;
        set
        {
            _width = value;
            if (Engine.IsEditorHint())
                GenerateMap();
        }
    }

    [Export]
    public int height
    {
        get => _height;
        set
        {
            _height = value;
            if (Engine.IsEditorHint())
                GenerateMap();
        }
    }

    [Export]
    public int seed
    {
        get => _seed;
        set
        {
            _seed = value;
            if (Engine.IsEditorHint())
                GenerateMap();
        }
    }

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
            GenerateMap();
    }

    public virtual void GenerateMap()
    {        
        Clear();
        noise.Seed = _seed;
        var sourceId = TileSet.GetSourceId(0);
        for (var x = 0; x < _width; x++)
            for (var y = 0; y < _height; y++)
            {
                var value = noise.GetNoise2D(x, y);
                var atlasCoords = value > 0.2 ? new Vector2I(1, 1) :
                                  value > -0.2 ? new Vector2I(0, 0) :
                                  new Vector2I(0, 0);
                SetCell(new Vector2I(x, y), sourceId, atlasCoords);
            }
    }

    public Vector2I GetTilePos() =>
        LocalToMap(ToLocal(GetGlobalMousePosition()));
}