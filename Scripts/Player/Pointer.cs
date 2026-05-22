using Godot;
using System;
using GameProject;

public partial class Pointer : Panel
{
	// Called when the node enters the scene tree for the first time.
	private Map map;
	private Vector2 startPos; 
	private Vector2 shift;
	private bool isRemoveMode;
	private StyleBoxFlat styleBox;
	public override void _Ready()
	{
		map = GetParent<Map>();
		startPos = Position;
		styleBox = GetThemeStylebox("panel") as StyleBoxFlat;
	}

	public void ChangeMode(bool isRemoveMode, BuildingData currentStructure)
	{
		this.isRemoveMode = isRemoveMode;
		if (isRemoveMode) styleBox.BorderColor = new Color(0xFF0000FF);
		else {
			styleBox.BorderColor = new Color(0xFFFF00FF);
			ChangePointer(currentStructure);
		}
	}

	public void ChangeMode(PlayerMode mode, BuildingData cs)
	{
		isRemoveMode = false;
		Visible = true;
		switch (mode)
		{
			case PlayerMode.Build:
				ChangePointer(cs);
				styleBox.BorderColor = new Color(0xFFFF00FF);
				break;
			case PlayerMode.Destroy:
				ResetPointer();
				styleBox.BorderColor = new Color(0xFF0000FF);
				isRemoveMode = true;
				break;
			default:
				Visible = false;
				break;
		}
	}

	public void ChangePointer(BuildingData structure)
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
			var info = BuildingManager.Instance.GetEntityAt(map.GlobalToMap(GetGlobalMousePosition()));
			if (info == null)
			{
				Position = startPos + map.GetTilePos() * 32;
				Size = Vector2.One * 36;
			}
			else
			{
				Position = startPos + info.StartPos * 32;
				Size = new Vector2(4 + Math.Max(1, info.Entity.Data.Size.X) * 32, 4 + Math.Max(1, info.Entity.Data.Size.X) * 32);
			}
		}
	}
}
