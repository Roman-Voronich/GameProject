using System;
using Godot;
namespace GameProject;

public interface IMovable
{
    float Speed { get; }
    bool IsMoving { get; }
    void MoveTo(Vector2 target);
    void StopMoving();
}