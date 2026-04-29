using Godot;
using System;

public partial class SkillBar : Node2D
{
	[Export] public HBoxContainer Container { get; set; }
	[Export] public SkillData[] Skills { get; set; } = new SkillData[0];

	private PackedScene _slotScene = GD.Load<PackedScene>("res://Scenes/UI/SkillSlot.tscn");

	public override void _Ready()
	{
		ReBuild();
	}


	public void ReBuild()
	{
		if (Container == null) return;
		foreach (var child in Container.GetChildren())
		{
			Container.RemoveChild(child);
			child.QueueFree();
		}

		foreach (var skill in Skills)
		{
			if (skill == null) continue;

			var slot = _slotScene.Instantiate<SkillSlot>();
			Container.AddChild(slot);

			slot.Setup(skill);
			slot.SkillPressed += OnSlotPressed;
		}
	}

	private void OnSlotPressed(SkillData skill)
	{
		GD.Print($"[SkillBar] Клик по: {skill.Name}");
		// Здесь позже будет вызов логики игрока
	}
}
