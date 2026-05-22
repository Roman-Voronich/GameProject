using Godot;
using System;

namespace GameProject;

public class BuildingEntity : IEntity
{
    // ---- IEntity ----
    public string Name { get; private set; }
    public Vector2 GlobalPosition => _structureMap?.MapToLocal(_gridPos) ?? Vector2.Zero; // это только относительно самой карты
    
    public int MaxHealth { get; private set; }
    public bool IsAlive => _currentHealth > 0;
    
    public EntityFaction Faction { get; private set; }
    
    // ---- Состояние ----
    private int _currentHealth;
    private Vector2I _gridPos;
    private TileMapLayer _structureMap;
    private IBuildingLogic _logic;

    public BuildingData Data;
    // ---- События (опционально, для UI) ----
    public event Action<BuildingEntity> OnHealthChanged;
    public event Action<BuildingEntity> OnDestroyed;

    public BuildingEntity(Vector2I gridPos, BuildingData data, TileMapLayer structureMap, IBuildingLogic logic, EntityFaction faction)
    {
        _gridPos = gridPos;
        _structureMap = structureMap;
        _logic = logic;
        Data = data;
        
        Name = data.Name;
        Faction = faction;
        MaxHealth = 100; // Можно вынести в BuildingData
        _currentHealth = MaxHealth;
        
        // Инициализируем логику, передавая ссылку на себя
        _logic.OnPlaced(gridPos, data, this);
    }

    public void TakeDamage(int amount, IEntity source = null)
    {
        if (!IsAlive) return;
        
        _currentHealth = Math.Max(0, _currentHealth - amount);
        OnHealthChanged?.Invoke(this);
        
        GD.Print($" {Name} получил {amount} урона. Осталось: {_currentHealth}/{MaxHealth}");
        
        if (!IsAlive)
        {
            Die(source);
        }
    }

    public void Heal(int amount, IEntity source = null)
    {
        if (!IsAlive) return;
        
        _currentHealth = Math.Min(MaxHealth, _currentHealth + amount);
        OnHealthChanged?.Invoke(this);
        
        GD.Print($" {Name} вылечен на {amount}. Здоровье: {_currentHealth}/{MaxHealth}");
    }

    public bool IsEnemy(IEntity other)
    {
        if (other == null) return false;
        return Faction != other.Faction;
    }

    private void Die(IEntity killer)
    {
        GD.Print($" {Name} уничтожен!");
        OnDestroyed?.Invoke(this);
        _logic.OnDestroyed();
    }

    // ---- Для менеджера ----
    public IBuildingLogic Logic => _logic;
    public Vector2I GridPosition => _gridPos;
    
    public void UpdateLogic(double delta) => _logic?.OnTick(delta);
    public void OnInteract() => _logic?.OnInteract();
}