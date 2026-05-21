using Godot;
using System.Collections.Generic;

namespace GameProject;

public partial class BuildingManager : Node
{
    public static BuildingManager Instance { get; private set; }

    private TileMapLayer _structureMap;
    private Dictionary<Vector2I, BuildingEntity> _entities = new();

    public override void _Ready()
    {
        Instance = this;
        CallDeferred(nameof(AutoFindMap));
    }

    private void AutoFindMap()
    {
        _structureMap = GetNode<TileMapLayer>("/root/Game/Map/StructureMap");
        
        if (_structureMap == null)
            GD.PrintErr("BuildingManager: StructureMap не найден!");
    }

    /// 🏗 Разместить здание
    public bool PlaceBuilding(Vector2I gridPos, BuildingData data, EntityFaction faction = EntityFaction.Player)
    {
        if (_structureMap == null || data == null) return false;
        if (!IsSpaceFree(gridPos, data.Size)) return false;

        // 1. Визуал
        MarkOccupied(gridPos, data.Size, data.TileId);

        // 2. Логика
        var logic = CreateLogic(data.Name);
        if (logic == null)
        {
            ClearOccupied(gridPos, data.Size);
            return false;
        }

        // 3. Сущность
        var entity = new BuildingEntity(gridPos, data, _structureMap, logic, faction);
        entity.OnDestroyed += OnEntityDestroyed;
        _entities[gridPos] = entity;

        GD.Print($"{data.Name} на {gridPos}");
        return true;
    }

    public void RemoveBuilding(Vector2I gridPos)
    {
        if (!_entities.TryGetValue(gridPos, out var entity)) return;
        
        entity.OnDestroyed -= OnEntityDestroyed;
        entity.Logic.OnDestroyed();
        ClearOccupied(gridPos, entity.Data.Size);
        _entities.Remove(gridPos);
    }

    public void UpdateBuildings(double delta)
    {
        foreach (var entity in _entities.Values)
            if (entity.IsAlive) entity.UpdateLogic(delta);
    }

    public void OnBuildingClicked(Vector2 worldPos)
    {
        if (_structureMap == null) return;
        var gridPos = _structureMap.LocalToMap(worldPos);
        GetEntityAt(gridPos)?.OnInteract();
    }


    private BuildingEntity GetEntityAt(Vector2I tilePos)
    {
        if (_entities.TryGetValue(tilePos, out var entity)) return entity;
        
        // Поиск по многотайловым зданиям
        foreach (var kvp in _entities)
        {
            var start = kvp.Key;
            var size = kvp.Value.Data.Size;
            if (tilePos.X >= start.X && tilePos.X < start.X + size.X &&
                tilePos.Y >= start.Y && tilePos.Y < start.Y + size.Y)
                return kvp.Value;
        }
        return null;
    }

    private bool IsSpaceFree(Vector2I start, Vector2I size)
    {
        for (int x = 0; x < size.X; x++)
            for (int y = 0; y < size.Y; y++)
                if (_structureMap.GetCellSourceId(start + new Vector2I(x, y)) != -1)
                    return false;
        return true;
    }

    private void MarkOccupied(Vector2I start, Vector2I size, int tileId)
    {
        for (int x = 0; x < size.X; x++)
            for (int y = 0; y < size.Y; y++)
                _structureMap.SetCell(start + new Vector2I(x, y), tileId, new Vector2I(x, y));
    }

    private void ClearOccupied(Vector2I start, Vector2I size)
    {
        for (int x = 0; x < size.X; x++)
            for (int y = 0; y < size.Y; y++)
                _structureMap.SetCell(start + new Vector2I(x, y), -1);
    }

    private void OnEntityDestroyed(BuildingEntity entity) => RemoveBuilding(entity.GridPosition);

    private IBuildingLogic CreateLogic(string name) => name switch
    {
       "Mine" => new MineLogic(),
        _ => null
    };
}