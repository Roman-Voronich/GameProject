using Godot;
using System.Collections.Generic;

namespace GameProject;

public partial class BuildingManager : Node
{
    public static BuildingManager Instance { get; private set; }

    private StructureMap _structureMap;
    private Map _map;
    private Dictionary<Vector2I, TileInfo> _entities = new();

    public override void _Ready()
    {
        Instance = this;
        CallDeferred(nameof(AutoFindMap));
        CallDeferred(nameof(placeMain));
    }

    private void placeMain()
    {
        var data = GD.Load<BuildingData>("res://Resources/Buildings/MainBuilding.tres");
        PlaceBuilding(new Vector2I(Map.width/2 -2 , Map.height/2 -2),data,EntityFaction.Player);
    }

    private void AutoFindMap()
    {
        _structureMap = GetNode<StructureMap>("/root/Game/Map/StructureMap");
        _map = _structureMap.GetParent<Map>();
        if (_structureMap == null)
            GD.PrintErr("BuildingManager: StructureMap не найден!");
    }

    ///Разместить здание
    public bool PlaceBuilding(Vector2I gridPos, BuildingData data, EntityFaction faction = EntityFaction.Player)
    {
        if (_structureMap == null || data == null) return false;
        if (!Map.CanPlaceBuilding(gridPos, data.Size)) return false;

        // 1. Визуал
        var logic = CreateLogic(data.Name);
        if (logic == null)
        {
            return false;
        }
        var entity = new BuildingEntity(gridPos, data, _structureMap, logic, faction);
        entity.OnDestroyed += OnEntityDestroyed;
        MarkOccupied(gridPos, entity);

        GD.Print($"{data.Name} на {gridPos}");
        for (int i = 0; i < 4; i++)
        {
            Inventory.ChangeCountResource((ResourceType)i,-data.Cost[i]);
            if (!Inventory.HaveResource((ResourceType)i, data.Cost[i])) Player.Mode = PlayerMode.Nothing;
        }
        return true;
    }

    public void RemoveBuilding(Vector2I gridPos)
    {
        if (!_entities.TryGetValue(gridPos, out var info)) return;
        if (info.Entity.Data.Name == "MainBuilding") return;
        info.Entity.OnDestroyed -= OnEntityDestroyed;
        info.Entity.Logic.OnDestroyed();
        ClearOccupied(info.StartPos, info.Entity.Data.Size);
    }

    public void UpdateBuildings(double delta)
    {
        foreach (var info in _entities.Values)
            if (info.Entity.IsAlive) info.Entity.UpdateLogic(delta);
    }

    public void OnBuildingClicked(Vector2 worldPos)
    {
        if (_structureMap == null) return;
        var gridPos = _structureMap.LocalToMap(worldPos);
        GetEntityAt(gridPos)?.Entity.OnInteract();
    }


    public TileInfo GetEntityAt(Vector2I tilePos)
    {
        _entities.TryGetValue(tilePos, out var info);
        return info;
    }

    private void MarkOccupied(Vector2I start, BuildingEntity entity)
    {
        for (int x = 0; x < entity.Data.Size.X; x++)
            for (int y = 0; y < entity.Data.Size.Y; y++)
            {
                _entities.Add(start + new Vector2I(x, y), new TileInfo(entity,start));
                _structureMap.SetCell(start + new Vector2I(x, y), entity.Data.TileId, new Vector2I(x, y));
            }
    }

    private void ClearOccupied(Vector2I start, Vector2I size)
    {
        for (int x = 0; x < size.X; x++)
        for (int y = 0; y < size.Y; y++)
        {
            _entities.Remove(start + new Vector2I(x, y));
            _structureMap.EraseCell(start + new Vector2I(x, y));
        }

    }

    public class TileInfo
    {
        public BuildingEntity Entity;
        public Vector2I StartPos;

        public TileInfo(BuildingEntity entity, Vector2I startPos)
        {
            Entity = entity;
            StartPos = startPos;
        }
    }

    private void OnEntityDestroyed(BuildingEntity entity) => RemoveBuilding(entity.GridPosition);

    private IBuildingLogic CreateLogic(string name) => name switch
    {
       "StoneMine" => new StoneMineLogic(),
       "Lumber" => new  LumberLogic(),
       "CopperMine" => new CopperMineLogic(),
       "IronMine" => new IronMine(),
       "MainBuilding" => new MainBuildingLogic(),
       "Tent" => new TentLogic(),
        _ => null
    };
}