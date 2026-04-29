using Godot;
using System;

[Tool, GlobalClass]
public partial class MapFromNoise : TileMapLayer
{
    private NoiseTexture2D _noiseTexture;
    private int _width = 50;
    private int _height = 50;
    private int _seed = 10;
    public TileInfo[] tiles = [
        new TileInfo("grass", 0, new Vector2I(1, 0)),
        new TileInfo("water", 0, new Vector2I(1, 1)),
        new TileInfo("tree", 0, new Vector2I(0, 0)),
        new TileInfo("pebbles", 0, new Vector2I(0, 1))
    ];

    [Export]
    public NoiseTexture2D noiseTexture
    {
        get => _noiseTexture;
        set
        {
            _noiseTexture = value;
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
            GenerateMap();
        }
    }

    public override void _Ready()
    {
        ZIndex = -30;
        GenerateMap();
    }

    private void GenerateMap()
    {        
        Clear();
        var noise = _noiseTexture.Noise as FastNoiseLite;
        noise.Seed = _seed;
        for (var x = 0; x < _width; x++)
            for (var y = 0; y < _height; y++)
            {
                var value = noise.GetNoise2D(x, y);
                var tileInfo = value > 0.2 ? tiles[1] :
                                  value > -0.2 ? tiles[0] :
                                  tiles[2];
                SetCell(new Vector2I(x, y), tileInfo.atlasId, tileInfo.atlasCoord);
            }
    }
}