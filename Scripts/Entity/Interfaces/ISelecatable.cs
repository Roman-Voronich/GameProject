using System;
using Godot;
namespace GameProject;

public interface ISelectable
{
    bool IsSelected { get; set; }
    bool CanBeSelected { get; }
    event Action<ISelectable, bool> OnSelectionChanged;
    void UpdateSelectionVisual();
}