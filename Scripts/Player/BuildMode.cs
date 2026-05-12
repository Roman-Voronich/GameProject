using Godot;
using System;

public partial class Player
{
    private Map map;
    private bool isBuildMode = false;
    private bool isRemoveMode = false;
    private bool isManyChange = false;
    private Pointer pointer;

    private void ChangeMap()
	{
        if (isRemoveMode) DestroyStructure();
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

    private void ChangeMode()
    {
        isRemoveMode = false;
        isManyChange = false;
        isBuildMode = !isBuildMode;
        if (isBuildMode) pointer.ChangePointer(currentStructure);
        else pointer.ResetPointer();
    }

    private void DoBuild()
	{
        pointer.ChangeMode(isRemoveMode, currentStructure);
		if (Input.IsActionJustPressed("ui_left_click")
            || (isManyChange
                && Input.IsActionPressed("ui_left_click"))
            ) ChangeMap();
	}
}
