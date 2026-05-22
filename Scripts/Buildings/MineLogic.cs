using Godot;

namespace GameProject;

public class MineLogic : IBuildingLogic
{
    public void OnPlaced(Vector2I gridPos, BuildingData data, BuildingEntity entity)
    {
        Inventory.AddPassiveIncome(ResourceType.Stone, 12);
    }

    public void OnTick(double delta)
    {
        
    }

    public void OnInteract()
    {
        
    }

    public void OnDestroyed()
    {
        Inventory.AddPassiveIncome(ResourceType.Stone, -12);
    }

    public string BuildingName { get; } = "Mine";
}