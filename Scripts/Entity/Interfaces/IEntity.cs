using Godot;
using System;
using System.Collections.Generic;

namespace GameProject;
public interface IEntity
{
	// ---- Идентификация ----
	string Name { get; }
	Vector2 GlobalPosition { get; }
    
	// ---- Здоровье ----
	int MaxHealth { get; }
	bool IsAlive { get; }
    
	void TakeDamage(int amount, IEntity source = null);
	void Heal(int amount, IEntity source = null);
    
	// ---- Фракция ----
	EntityFaction Faction { get; }
	bool IsEnemy(IEntity other);
	
}

public enum EntityFaction
{
	Player,
	Enemy
}