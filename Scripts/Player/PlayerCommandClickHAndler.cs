using Godot;
using System;
using System.Collections.Generic;

namespace GameProject;
public partial class PlayerCommandClickHAndler : Node2D
{

	private List<Mob> _selectedMobs = new List<Mob>();
	private Random _random = new Random();

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			GD.Print(_selectedMobs.Count, "перед очисткой");
			RemoveDeadMobs();
			GD.Print(_selectedMobs.Count, "после очистки"); 
			Vector2 mousePosition = GetViewport().GetCamera2D().GetGlobalMousePosition();
			
			var parameters = new PhysicsPointQueryParameters2D();
			parameters.Position = mousePosition;
			parameters.CollisionMask = 2;
			var results = GetTree().Root.GetWorld2D().DirectSpaceState.IntersectPoint(parameters);
			Mob clickedMob = null;
			results.Reverse();
			foreach (var result in results)
			{
				if (result["collider"].Obj is Mob mob)
				{
					clickedMob = mob;
					break;
				}
			}

			if (mouseButton.ButtonIndex == MouseButton.Right)
			{
				if (clickedMob != null)
				{
					if (clickedMob.Faction == EntityFaction.Player)
					{
						if (!_selectedMobs.Contains(clickedMob))
						{
							_selectedMobs.Add(clickedMob);
							clickedMob.Select();
						}
						else
						{
							_selectedMobs.Remove(clickedMob);
							clickedMob.Unselect();
						}
					}
					else if (clickedMob.Faction == EntityFaction.Enemy)
					{
						foreach (var mob in _selectedMobs)
						{
							mob.SetUpTarget(clickedMob);
							mob.Unselect();
						}
						_selectedMobs.Clear();
					}
				}
				else
				{
					foreach (var mob in _selectedMobs)
					{
						mob.SetOffTarget();
						Vector2 offset = new Vector2(
							(float)(_random.NextDouble() * 80 - 40),
							(float)(_random.NextDouble() * 80 - 40)
						);
						mob.MoveTo(mousePosition + offset);
						mob.Unselect();
					}
					_selectedMobs.Clear();
				}
			}

			else if (mouseButton.ButtonIndex == MouseButton.Left && clickedMob == null)
			{
				foreach (var mob in _selectedMobs)
				{
					mob.SetOffTarget();
					Vector2 offset = new Vector2(
						(float)(_random.NextDouble() * 80 - 40),
						(float)(_random.NextDouble() * 80 - 40));
					mob.MoveTo(mousePosition + offset);
					mob.Unselect();
				}
				_selectedMobs.Clear();
			}
		}
	}
	private void RemoveDeadMobs()
	{
		_selectedMobs.RemoveAll(mob => !IsInstanceValid(mob) || mob.IsQueuedForDeletion());
	}
}
