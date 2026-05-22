using Godot;
using System;
using GameProject;

public partial class Player
{
    public static event Action<PlayerMode> ModeChanged;
    private static PlayerMode _mode;

    public static PlayerMode Mode
    {
        get => _mode;
        set
        {
            if (_mode == value) return;
            _mode = value;

            ModeChanged?.Invoke(value);
        }
    }
    private bool isManyChange = false;

    private void ChangeMap()
	{
        if (Mode == PlayerMode.Destroy) DestroyStructure();
		else BuildStructure();
	}

    private void BuildStructure()
    {
        var temp = GetGlobalMousePosition() - new Vector2I(currentStructure.Size.X - 1, currentStructure.Size.Y - 1) * 16;
        var startPos = Map.GlobalToMap(temp);
        BuildingManager.Instance.PlaceBuilding(startPos, currentStructure);

    }

    private void DestroyStructure()
    {
        var coord = Map.GlobalToMap(GetGlobalMousePosition());
        BuildingManager.Instance.RemoveBuilding(coord);
    }

    private void DoBuild()
	{
		if (Input.IsActionJustPressed("ui_left_click")
            || (isManyChange
                && Input.IsActionPressed("ui_left_click"))
            ) ChangeMap();
	}
}
