using Godot;

namespace GameProject;

public class CopperMineLogic : IBuildingLogic
{
    public void OnPlaced(Vector2I gridPos, BuildingData data, BuildingEntity entity)
    {
        Inventory.AddPassiveIncome(ResourceType.Copper, 9);
    }

    public void OnTick(double delta)
    {
    }

    public void OnInteract()
    {
    }

    public void OnDestroyed()
    {
        Inventory.AddPassiveIncome(ResourceType.Copper, -9);
    }

    public string BuildingName { get; } = "CopperMine";
}