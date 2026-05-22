using System;
using System.Collections.Generic;
using Godot;

public partial class Inventory : Node
{
    public static event Action<ResourceType, int> InventoryChange;
    private static Rational[] inventory;
    private static Rational[] passiveIncome;
    private static bool havePassiveIncome = false;
    private static int tps;

    public override void _Ready()
    {
        tps = Engine.PhysicsTicksPerSecond;
        var countRes = Enum.GetNames<ResourceType>().Length;
        inventory = new Rational[countRes];
        passiveIncome = new Rational[countRes];
        for (var i = 0; i < countRes; i++)
        {
            inventory[i] = new Rational(tps * 60);
            /*if (EngineDebugger.IsActive())*/ inventory[i] += 1000;
            passiveIncome[i] = new Rational(tps * 60);
        }
    }

    public static void ChangeCountResource(ResourceType resource, int count)
    {
        if (count == 0) return;
        inventory[(int)resource].Whole += count;
        InventoryChange?.Invoke(resource, count);
    }

    public static bool HaveResource(ResourceType res, int count) =>
        inventory[(int)res] >= count;

    public static bool HaveResources(ResourceType[] resources, int[] counts)
    {
        if (resources.Length != counts.Length || resources.Length > inventory.Length) return false;
        for (var i = 0; i < resources.Length; i++)
            if (!HaveResource(resources[i], counts[i])) return false;
        return true;
    }


    public static void AddPassiveIncome(ResourceType res, int resPerMinute)
    {
        passiveIncome[(int)res].AddFraction(resPerMinute);
        havePassiveIncome = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!havePassiveIncome) return;
        for (var i = 0; i < passiveIncome.Length; i++)
        {
            inventory[i] += passiveIncome[i];
            if (passiveIncome[i].Whole != 0
                || (passiveIncome[i].Fraction > inventory[i].Fraction && inventory[i] > 0))
            {
                InventoryChange.Invoke((ResourceType)i, 0);
            }
        }
    }

    public static Rational GetCountResource(ResourceType res) => inventory[(int)res];
}