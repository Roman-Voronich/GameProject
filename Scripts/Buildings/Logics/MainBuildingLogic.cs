using Godot;
using GodotPlugins.Game;

namespace GameProject;

public class MainBuildingLogic :IBuildingLogic
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
        Game.Lose();
    }

    public string BuildingName { get; } = "MainBuilding";
}