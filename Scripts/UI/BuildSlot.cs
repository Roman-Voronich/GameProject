using Godot;
using System;

public partial class BuildSlot : Button
{
	[Signal]
	public delegate void SlotClickedEventHandler(BuildingData building);
	private TextureRect _icon;
	private VBoxContainer _costs;
	private BuildingData _data;
	private Label _name;
	public override void _Ready()
	{
		_icon = GetNode<TextureRect>("Icon");
		_costs = GetNode<VBoxContainer>("Costs");
		_name = GetNode<Label>("Name");
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}
	
	public void Setup(BuildingData data)
	{
		_data = data;
		if(_icon == null) GD.PrintErr("Icon not found");
		_icon.Texture = data.Icon;
		_name.Text = data.Name;
		foreach (var child in _costs.GetChildren())
		{
			if (child.Name == "wood")
				child.GetNode<Label>("Label").Text = data.Cost[(int)ResourceType.Wood].ToString();
			else if(child.Name == "stone")
				child.GetNode<Label>("Label").Text = data.Cost[(int)ResourceType.Stone].ToString();
			else if(child.Name == "copper")
				child.GetNode<Label>("Label").Text = data.Cost[(int)ResourceType.Copper].ToString();
			else if(child.Name == "iron")
				child.GetNode<Label>("Label").Text = data.Cost[(int)ResourceType.Iron].ToString();
		}
	}
	private void OnMouseEntered()
	{
		if (_data.Description != null)
		{
			Tooltip.Instance?.Show(_data.Description, GetGlobalMousePosition());
		}
	}

	private void OnMouseExited()
	{
		Tooltip.Instance?.Hide();
	}

	public override void _Pressed()
	{
		for (int i = 0; i < 4; i++)
		{
			if (!Inventory.HaveResource((ResourceType)i, _data.Cost[i])) return;

		}
		EmitSignal(SignalName.SlotClicked, _data);
	}
}
