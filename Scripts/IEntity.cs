using Godot;
using System;
using System.Collections.Generic;

namespace GameProject;
public interface IEntity
{
	// ---- Идентификация ----
	string Name { get; }
	EntityType Type { get; }
	Vector2 GlobalPosition { get; }
	Node2D EntityNode { get; }
    
	// ---- Здоровье ----
	float MaxHealth { get; }
	float CurrentHealth { get; }
	bool IsAlive { get; }
    
	void TakeDamage(float amount, IEntity source = null);
	void Heal(float amount);
    
	// ---- Фракция ----
	EntityFaction Faction { get; }
	bool IsEnemy(IEntity other);
	bool IsAlly(IEntity other);
    
	// ---- События ----
	event Action<IEntity> OnDeath;
	event Action<IEntity, float, float> OnHealthChanged;
}

public enum EntityType
{
	Mob,
	Building,
	Decoration,
	Resource
}

public enum EntityFaction
{
	Player,
	Enemy,
	Neutral,
	Ally
}