using Godot;
using System;
using System.Collections.Generic;
using GameProject.Scripts.Skills;

public partial class SkillSystem : Node
{
	private Dictionary<string, ISkillAction> _actions = new();
	private Player _player;
	private SkillBar _skillBar;
	
	private void AddActions()
	{
		_actions["Build"] = new BuildAction();
		_actions["Destroy"] = new DestroyAction();
	}
	public override void _Ready()
	{
		AddActions();
		CallDeferred(nameof(ConnectSkillBar));
		CallDeferred(nameof(ConnectPlayer));
		GD.Print(_player);
	}

	private void ConnectSkillBar()
	{
		_skillBar = GetTree().GetFirstNodeInGroup("SkillBar") as SkillBar;
		
		GD.Print($"SkillSystem Подключён к SkillBar");
		_skillBar.SkillUsed += SkillUse;
	}
	private void ConnectPlayer()
	{
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		GD.Print($"SkillSystem Подключён к Player");
	}
	Vector2 ChooseWorldPos()
	{
		throw new NotImplementedException();
	}

	void SkillUse(SkillData skill, bool isToggledOn)
	{
		//дописать еще проверку на кулдаун
		var worldPos = new Vector2();
		if (skill.IsTargetable) worldPos = ChooseWorldPos();
		Cast(skill, worldPos);
		
	}

	void Cast(SkillData skill, Vector2 worldPos)
	{
		if (!_actions.TryGetValue(skill.Name, out var action))
		{
			GD.PrintErr($"No logic registered for skill: {skill.Name}");
			return;
		}

		if (!action.CanCast(skill, worldPos, _player))
		{
			GD.Print($"Cant cast {skill.Name}");
			return;
		}
		action.Cast(skill, worldPos, _player);
	}
	
	public override void _Process(double delta)
	{
	}
}
