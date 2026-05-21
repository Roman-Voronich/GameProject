using Godot;

public interface IBuildingLogic
{
    void OnPlaced(Vector2I gridPos, BuildingData data);
    
    void OnTick(double delta);
    
    void OnInteract();
    
    void OnDestroyed();
    string BuildingName { get; }
    
}