using System.Collections.Generic;

public partial class Player
{
    private Dictionary<string, int> inventory;

    public void AddResource(string resource, int count)
    {
        if (!inventory.TryAdd(resource, count))
            inventory[resource] += count;
    }
}