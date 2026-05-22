using Godot;

namespace GameProject;

public class MineLogic : IBuildingLogic
{
    public void OnPlaced(Vector2I gridPos, BuildingData data, BuildingEntity entity)
    {
        GD.Print("Mine placed");
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

    public string BuildingName { get; } = "Mine";
}