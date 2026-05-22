using Godot;

namespace GameProject;

public class IronMine : IBuildingLogic
{
    public void OnPlaced(Vector2I gridPos, BuildingData data, BuildingEntity entity)
    {
        Inventory.AddPassiveIncome(ResourceType.Iron, 5);
    }

    public void OnTick(double delta)
    {
    }

    public void OnInteract()
    {
    }

    public void OnDestroyed()
    {
        Inventory.AddPassiveIncome(ResourceType.Iron, -5);
    }

    public string BuildingName { get; } = "IronMine";
}