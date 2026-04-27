using Godot;
using System;

[Tool, GlobalClass]
public partial class MapFromNoise : TileMapLayer
{
	private NoiseTexture2D _noiseTexture;
	private int _width = 50;
	private int _height = 50;
	private int _seed = 10;

	[Export]
	public NoiseTexture2D noiseTexture
	{
		get => _noiseTexture;
		set
		{
			_noiseTexture = value;
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

	private void GenerateMap()
	{        
		Clear();
		var noise = _noiseTexture.Noise as FastNoiseLite;
		noise.Seed = _seed;
		for (var x = 0; x < _width; x++)
			for (var y = 0; y < _height; y++)
			{
				var value = noise.GetNoise2D(x, y);
				var atlasCoords = value > 0.2 ? new Vector2I(1, 1) :
								  value > -0.2 ? new Vector2I(1, 0) :
								  new Vector2I(0, 0);
				SetCell(new Vector2I(x, y), 0, atlasCoords);
			}
	}
}
