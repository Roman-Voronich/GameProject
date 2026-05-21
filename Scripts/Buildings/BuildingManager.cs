using Godot;
using System.Collections.Generic;

public partial class BuildingManager : Node
{
    public static BuildingManager Instance { get; private set; }

    private TileMapLayer _structureMap;
    
    private Dictionary<Vector2I, BuildingRecord> _registry = new();

    public override void _Ready()
    {
        Instance = this;
        CallDeferred(nameof(AutoFindMap));
    }

    private void AutoFindMap()
    {
        _structureMap = GetNode<TileMapLayer>("/root/Game/Map/StructureMap");
    }

    public bool PlaceBuilding(Vector2I gridPos, BuildingData data)
    {
        if (_structureMap == null || data == null) return false;
        if (!IsSpaceFree(gridPos, data.Size))
        {
            GD.Print($"Место занято или выходит за границы: {gridPos}");
            return false;
        }

        // 1. Визуал: ставим тайлы на карту
        MarkOccupied(gridPos, data.Size, data.TileId);

        // 2. Логика: создаём поведение по имени
        var logic = CreateLogic(data.Name);
        if (logic == null)
        {
            GD.PrintErr($"Нет логики для здания с Id: {data.Name}");
            ClearOccupied(gridPos, data.Size); // Откат визуала
            return false;
        }

        // 3. Связь: инициализируем и сохраняем в реестр
        logic.OnPlaced(gridPos, data);
        _registry[gridPos] = new BuildingRecord { Logic = logic, Size = data.Size };

        GD.Print($"Построено: {data.Name} на {gridPos}");
        return true;
    }

    public void RemoveBuilding(Vector2I gridPos)
    {
        if (!_registry.TryGetValue(gridPos, out var record)) return;

        record.Logic.OnDestroyed();
        _registry.Remove(gridPos);
        ClearOccupied(gridPos, record.Size);
        
        GD.Print($"Удалено здание на {gridPos}");
    }
//вызывать в Process главной ноды сцены или по таймеру
    public void UpdateBuildings(double delta)
    {
        foreach (var record in _registry.Values)
        {
            record.Logic.OnTick(delta);
        }
    }

    ///Обработка клика по миру
    public void OnBuildingClicked(Vector2 worldPos)
    {
        if (_structureMap == null) return;

        Vector2I tilePos = _structureMap.LocalToMap(worldPos);
        var record = GetBuildingAt(tilePos);

        if (record != null)
        {
            record.Logic.OnInteract();
        }
    }


    private BuildingRecord GetBuildingAt(Vector2I tilePos)
    {
        // Прямой поиск (если клик в верхний-левый угол)
        if (_registry.TryGetValue(tilePos, out var record)) return record;

        // Поиск по всем зарегистрированным зданиям (поддержка клика в центр многотайлового здания)
        foreach (var kvp in _registry)
        {
            var start = kvp.Key;
            var size = kvp.Value.Size;
            if (tilePos.X >= start.X && tilePos.X < start.X + size.X &&
                tilePos.Y >= start.Y && tilePos.Y < start.Y + size.Y)
            {
                return kvp.Value;
            }
        }
        return null;
    }

    private bool IsSpaceFree(Vector2I start, Vector2I size)
    {
        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
            {
                Vector2I tile = start + new Vector2I(x, y);
                if (_structureMap.GetCellSourceId(tile) != -1) return false;
            }
        }
        return true;
    }

    private void MarkOccupied(Vector2I start, Vector2I size, int tileId)
    {
        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
            {
               // _structureMap.SetCell(start + new Vector2I(x, y), tileId);
               // В MarkOccupied:
               _structureMap.SetCell(
                   start + new Vector2I(x, y), 
                   tileId, 
                    Vector2I.Zero +  new Vector2I(x, y), 
                   0
               );
               GD.Print($"📍 SetCell: pos={start}, src={tileId}, result={_structureMap.GetCellSourceId(start)}");
            }
        }
    }

    private void ClearOccupied(Vector2I start, Vector2I size)
    {
        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
            {
                _structureMap.SetCell(start + new Vector2I(x, y), -1);
            }
        }
    }

    private IBuildingLogic CreateLogic(string name)
    {
        return name switch
        {
            "farm"=> new FarmLogic(),
            /*"barracks" => new BarracksLogic(),
            "mine"     => new MineLogic(),
            _          => null*/
        };
    }

    private class BuildingRecord
    {
        public IBuildingLogic Logic { get; set; } = null!;
        public Vector2I Size { get; set; }
    }
}