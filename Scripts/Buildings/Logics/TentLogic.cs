using Godot;

namespace GameProject;

public class TentLogic : IBuildingLogic
{
    public void OnPlaced(Vector2I gridPos, BuildingData data, BuildingEntity entity)
    {
        
    }

    public void OnTick(double delta)
    {
        
    }

    public void OnInteract()
    {
        
    }

    public void OnDestroyed()
    {
        
    }

    public string BuildingName { get; } = "Tent";
}