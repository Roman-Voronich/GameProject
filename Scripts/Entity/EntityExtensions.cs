using Godot;
using System;
namespace GameProject;

public static class EntityExtensions
{
    public static float GetDistanceTo(this IEntity self, IEntity other)
    {
        return self.GlobalPosition.DistanceTo(other.GlobalPosition);
    }

    public static bool IsInRange(this IEntity self, IEntity other, float range)
    {
        return self.GlobalPosition.DistanceTo(other.GlobalPosition) <= range;
    }
}