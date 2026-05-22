using Godot;
using System;
namespace GameProject;

public partial class MeleeMob : Mob
{
    public override void _Ready()
    {
        var temp = GetNode<Area2D>("DetectionArea");
        temp.InputEvent += OnAreaClicked;
    }

    private void OnAreaClicked(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent && 
            mouseEvent.ButtonIndex == MouseButton.Left && 
            mouseEvent.Pressed)
        {
            GD.Print("Клик по персонажу!");
        }
        GD.Print(67);
    }
}