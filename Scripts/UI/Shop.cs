using Godot;
using System;

public partial class Shop : Control
{
	private GridContainer _grid;
	private Button _exitButton;
	private PackedScene _slot;
	[Export] BuildingData[]  buildings;
	public override void _Ready()
	{
		_grid = GetNode<GridContainer>("GridContainer");
		_exitButton = GetNode<Button>("ExitButton");
		_slot =  GD.Load<PackedScene>("res://Scenes/UI/BuildSlot.tscn");
		_exitButton.Pressed += ExitButtonPressed;
		foreach (var building in buildings)
		{
			var slot = _slot.Instantiate<BuildSlot>();
			slot._Ready();
			slot.Setup(building);
			slot.SlotClicked += OnSlotPressed;
			_grid.AddChild(slot);
		}
	}

	public void ExitButtonPressed()
	{
		GetParent().RemoveChild(this);
	}
	
	public void OnSlotPressed(BuildingData building)
	{
		Player player = GetTree().GetFirstNodeInGroup("Player") as Player;
		player.currentStructure = building;
		player.ChangeMode();
		ExitButtonPressed();
	}
}
