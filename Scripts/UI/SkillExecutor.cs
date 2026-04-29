using Godot;
using System;

public partial class SkillExecutor : Node
{
	public void Execute(SkillData data, Vector2 targetWorldPos)
	{
		//if (!_CanUseSkill(data.Name)) return;

		GD.Print($"Исполнен: {data.Name} | Позиция: {targetWorldPos}");
	}
}
