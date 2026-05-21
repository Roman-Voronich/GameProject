using Godot;
using System;
using GameProject;

public partial class Player
{
    private Map map;
    public bool isBuildMode = false;
    public bool isRemoveMode = false;
    private bool isManyChange = false;
    private Pointer pointer;

    private void ChangeMap()
	{
        if (isRemoveMode) DestroyStructure();
		else BuildStructure();
	}

    private void BuildStructure()
    {
        var temp = GetGlobalMousePosition() - new Vector2I(currentStructure.Size.X - 1, currentStructure.Size.Y - 1) * 16;
        var startPos = map.GlobalToMap(temp);
        BuildingManager.Instance.PlaceBuilding(startPos, currentStructure);

    }

    private void DestroyStructure()
    {
        var coord = map.GlobalToMap(GetGlobalMousePosition());
        BuildingManager.Instance.RemoveBuilding(coord);
    }

    public void ChangeMode()
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
