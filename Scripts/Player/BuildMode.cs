using Godot;
using System;

public partial class Player
{
    private Resource resourceMap;
	private TileMapLayer structureMap;
    private Terrian terrianMap;
    private bool isBuildMode = false;
    private bool isRemoveMode = false;
    private bool isManyChange = false;
    private Pointer pointer;

    private void ChangeMap()
	{
        if (isRemoveMode) RemoveTile();
		else BuildTile();
	}

    private void BuildTile()
    {
        var mouseGlobalPos = camera.GetGlobalMousePosition();
		var tilePos = structureMap.LocalToMap(structureMap.ToLocal(mouseGlobalPos));
        if (CanBuild(tilePos)) structureMap.SetCell(tilePos, 0, currentTile);
    }

    private bool CanBuild(Vector2I tilePos)
    {
        return structureMap.GetCellSourceId(tilePos) == -1
            && resourceMap.GetCellSourceId(tilePos) == -1
            && terrianMap.GetCellSourceId(tilePos) != -1
            && terrianMap.GetCellAtlasCoords(tilePos) != new Vector2I(1, 1);
    }

    private void RemoveTile()
    {
        var mouseGlobalPos = camera.GetGlobalMousePosition();
		var tilePos = structureMap.LocalToMap(structureMap.ToLocal(mouseGlobalPos));
        structureMap.EraseCell(tilePos);
    }

    private void ChangeMode()
    {
        isRemoveMode = false;
        isManyChange = false;
        isBuildMode = !isBuildMode;
        pointer.ChangeColor(isBuildMode, isRemoveMode);
    }

    private void DoBuild()
	{
        pointer.ChangeColor(isBuildMode, isRemoveMode);
		if (Input.IsActionJustPressed("ui_left_click")
            || (isManyChange
                && Input.IsActionPressed("ui_left_click"))
            ) ChangeMap();
	}
}
