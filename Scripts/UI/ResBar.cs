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
	public override void _Process(double delta)
	{
	}
}
