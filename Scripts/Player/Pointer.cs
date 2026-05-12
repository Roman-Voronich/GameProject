using Godot;
using System;

public partial class Pointer : Panel
{
	// Called when the node enters the scene tree for the first time.
	private Map map;
	private Vector2 startPos; 
	private Vector2 shift;
	private bool isRemoveMode; 
	public override void _Ready()
	{
		map = GetParent<Map>();
		startPos = Position;
	}

	public void ChangeMode(bool isRemoveMode, Structure currentStructure)
	{
		this.isRemoveMode = isRemoveMode;
		var styleBox = GetThemeStylebox("panel") as StyleBoxFlat;
		if (isRemoveMode) styleBox.BorderColor = new Color(0xFF0000FF);
		else {
			styleBox.BorderColor = new Color(0xFFFF00FF);
			ChangePointer(currentStructure);
		}
	}

	public void ChangePointer(Structure structure)
	{
		Size = new Vector2(4, 4) + structure.Size * 32;
		shift = (structure.Size - Vector2I.One) * -16;
	}

	public void ResetPointer()
	{
		var styleBox = GetThemeStylebox("panel") as StyleBoxFlat;
		styleBox.BorderColor = new Color(0xFFFFFFFF);
		Size = Vector2.One * 36;
		shift = Vector2.Zero;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!isRemoveMode) Position = startPos + map.GetTilePos(shift) * 32;
		else
		{
			var info = map.GetStructureInfo(map.GlobalToMap(GetGlobalMousePosition()));
			if (info.Z == 0)
			{
				Position = startPos + map.GetTilePos() * 32;
				Size = Vector2.One * 36;
			}
			else
			{
				Position = startPos + new Vector2(info.X, info.Y) * 32;
				Size = new Vector2(4 + Math.Max(1, info.Z) * 32, 4 + Math.Max(1, info.W) * 32);
			}
		}
	}
}
