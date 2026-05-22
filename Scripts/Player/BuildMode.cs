using Godot;
using System;

public partial class Player
{
    private Map map;
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
        var temp = GetGlobalMousePosition() - new Vector2I(currentStructure.Width - 1, currentStructure.Height - 1) * 16;
        var startPos = map.GlobalToMap(temp);
        map.TryBuildStructure(currentStructure, startPos);
    }

    private void DestroyStructure()
    {
        var pos = map.GetTilePos();
        var si = map.GetStructureInfo(pos);
        if (si.Z == 0) return;
        map.DestroyStructure(new Vector2I(si.X, si.Y), si.Z, si.W);
    }

    private void DoBuild()
	{
		if (Input.IsActionJustPressed("ui_left_click")
            || (isManyChange
                && Input.IsActionPressed("ui_left_click"))
            ) ChangeMap();
	}
}
