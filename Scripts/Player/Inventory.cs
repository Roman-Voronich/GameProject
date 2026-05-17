using System;
using System.Collections.Generic;
using Godot;

public partial class Player
{
    [Signal]
    public delegate void InventoryChangeEventHandler(string nameResource, int count);
    private Dictionary<string, int> inventory;

    public void ChangeCountResource(string resource, int count)
    {
        if (!inventory.TryAdd(resource, count))
            inventory[resource] += count;
        EmitSignal(SignalName.InventoryChange, resource, count);
    }

    public int GetCountResource(string resource)
    {
        inventory.TryGetValue(resource, out var count);
        return count;
    }
}