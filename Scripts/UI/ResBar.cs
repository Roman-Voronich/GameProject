using Godot;
using System;
using System.Collections.Generic;

public partial class ResBar : HBoxContainer
{
	[Export] public ResourceData[] Resources;
	[Export] public PackedScene SlotScene;
	private Player _player;
	public override void _Ready()
	{
		ReBuild();
		Inventory.InventoryChange += Update;
		Inventory.PassiveIncomeChange += UpdateIncome;
	}

	public void ReBuild()
	{
		foreach (var child in GetChildren())
		{
			RemoveChild(child);
			child.QueueFree();
		}

		foreach (var res in Resources)
		{
			if (res == null) continue;

			var slot = SlotScene.Instantiate<ResSlot>();
			AddChild(slot);
			
			slot.Setup(res,0); //Потом доставать из инвентаря, по сигналу изменения вызывать Update
		}
	}

	public void Update(ResourceType resource, int count)
	{
		foreach (ResSlot child in GetChildren())
		{
			if (child.Data.Name != Enum.GetName<ResourceType>(resource)) continue;
			child.Update(Inventory.GetCountResource(resource));
		}
	}

	public void UpdateIncome(ResourceType resource, float count)
	{
		foreach (ResSlot child in GetChildren())
		{
			if (child.Data.Name != Enum.GetName<ResourceType>(resource)) continue;
			child.UpdateIncome(Inventory.GetPassiveIncome(resource));
		}
	}

	public void TotalUpdate()
	{
		foreach (var res in Enum.GetValues<ResourceType>())
			Update(res, 0);
	}

	public override void _Process(double delta)
	{
	}
}
