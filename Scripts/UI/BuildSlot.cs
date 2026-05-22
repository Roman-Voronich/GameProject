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
				child.GetNode<Label>("Label").Text = data.WoodCost.ToString();
			else if(child.Name == "stone")
				child.GetNode<Label>("Label").Text = data.StoneCost.ToString();
			else if(child.Name == "copper")
				child.GetNode<Label>("Label").Text = data.CopperCost.ToString();
			else if(child.Name == "iron")
				child.GetNode<Label>("Label").Text = data.IronCost.ToString();
		}
	}

	public override void _Pressed()
	{
		EmitSignal(SignalName.SlotClicked, _data);
	}
}
