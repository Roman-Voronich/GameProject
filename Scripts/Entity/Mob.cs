using Godot;
using System;
using System.Collections.Generic;

namespace GameProject;

[GlobalClass]
public abstract partial class Mob : CharacterBody2D
{
    // ==================== ЭКСПОРТИРУЕМЫЕ ПОЛЯ ====================
    [Export] public EntityFaction Faction { get; set; } = EntityFaction.Enemy;
    [Export] public int MaxHealth { get; set; } = 100;
    [Export] public float Speed { get; set; } = 200f;
    [Export] public int Damage { get; set; } = 20;
    [Export] public float AttackRange { get; set; } = 50f;
    [Export] public float AttackCooldown { get; set; } = 1f;
    [Export] public float DetectionRange { get; set; } = 200f;
    
    // ==================== ПРИВАТНЫЕ ПОЛЯ ====================
    private int _currentHealth;
    private float _attackTimer;
    private MobState _currentState = MobState.Idle;
    private Mob _currentTarget;
    private List<Mob> _enemiesInRange = new List<Mob>();
    
    // ==================== КОМПОНЕНТЫ ====================
    private HealthBar _healthBar;
    private NavigationAgent2D _navAgent;
    private Area2D _detectionArea;
    private CollisionShape2D _collisionShape;
    private Sprite2D _sprite;
    
    // ==================== СИГНАЛЫ ====================
    [Signal] public delegate void DeathEventHandler();
    
    // ==================== GODOT МЕТОДЫ ====================
    public override void _Ready()
    {
        _currentHealth = MaxHealth;
        
        _navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        _detectionArea = GetNode<Area2D>("DetectionArea");
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        _healthBar = GetNode<HealthBar>("HealthBar");
        _sprite = GetNode<Sprite2D>("Sprite2D");
        
        // Настройка зоны обнаружения
        if (_detectionArea.GetNode<CollisionShape2D>("CollisionShape2D")?.Shape is CircleShape2D circle)
        {
            circle.Radius = DetectionRange;
        }

        _detectionArea.BodyEntered += OnBodyEntered;
        _detectionArea.BodyExited += OnBodyExited;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (_currentState == MobState.Death) return;
        
        // Обновляем список живых врагов
        int beforeCount = _enemiesInRange.Count;
        _enemiesInRange.RemoveAll(e => e == null || !e.IsAlive);
        
        switch (_currentState)
        {
            case MobState.Idle:
                ProcessIdle();
                break;
            case MobState.Chase:
                ProcessChase();
                break;
            case MobState.Attack:
                ProcessAttack(delta);
                break;
            case MobState.Move:
                ProcessMove();
                break;
        }
    }
    
    // ==================== ЛОГИКА СОСТОЯНИЙ ====================
    private void ProcessIdle()
    {
        
        if (_enemiesInRange.Count > 0)
        {
            _currentTarget = FindNearestEnemy();
            if (_currentTarget != null)
                _currentState = MobState.Chase;
        }
    }
    
    private void ProcessChase()
    {
        if (_currentTarget == null || !_currentTarget.IsAlive)
        {
            _currentTarget = FindNearestEnemy();
            if (_currentTarget == null)
            {
                _currentState = MobState.Idle;
                return;
            }
        }
        
        float distance = GlobalPosition.DistanceTo(_currentTarget.GlobalPosition);
        
        if (distance <= AttackRange)
        {
            _currentState = MobState.Attack;
            Velocity = Vector2.Zero;
        }
        else
        {
            _navAgent.TargetPosition = _currentTarget.GlobalPosition;
            Vector2 direction = (_navAgent.GetNextPathPosition() - GlobalPosition).Normalized();
            Velocity = direction * Speed;
            MoveAndSlide();
        }
    }
    
    private void ProcessAttack(double delta)
    {
        if (_currentTarget == null || !_currentTarget.IsAlive)
        {
            _currentTarget = FindNearestEnemy();
            if (_currentTarget == null)
            {
                _currentState = MobState.Idle;
                return;
            }
            _currentState = MobState.Chase;
            return;
        }
        
        float distance = GlobalPosition.DistanceTo(_currentTarget.GlobalPosition);
        
        if (distance > AttackRange + 20f)
        {
            _currentState = MobState.Chase;
            return;
        }
        
        _attackTimer += (float)delta;
        if (_attackTimer >= AttackCooldown)
        {
            _attackTimer = 0f;
            _currentTarget.TakeDamage(Damage, this);
            
            // Если цель умерла - сразу ищем нового
            if (!_currentTarget.IsAlive)
            {
                _currentTarget = FindNearestEnemy();
                if (_currentTarget == null)
                {
                    _currentState = MobState.Idle;
                }
                else
                {
                    _currentState = MobState.Chase;
                }
            }
        }
    }
    
    private void ProcessMove()
    {
        if (_enemiesInRange.Count > 0)
        {
            _currentTarget = FindNearestEnemy();
            _currentState = MobState.Chase;
            return;
        }
        
        if (_navAgent.IsNavigationFinished())
        {
            _currentState = MobState.Idle;
            Velocity = Vector2.Zero;
            return;
        }
        
        Vector2 direction = (_navAgent.GetNextPathPosition() - GlobalPosition).Normalized();
        Velocity = direction * Speed;
        MoveAndSlide();
    }
    
    // ==================== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ====================
    private void OnBodyEntered(Node2D body)
    {
        
        if (body == this)
        {
            return;
        }
        
        if (body is Mob mob)
            _enemiesInRange.Add(mob);
    }
    
    private void OnBodyExited(Node2D body)
    {
        
        if (body is Mob mob)
        {
            bool wasRemoved = _enemiesInRange.Remove(mob);
            if (_currentTarget == mob)
            {
                _currentTarget = FindNearestEnemy();
            }
        }
    }
    
    private Mob FindNearestEnemy()
    {
        
        Mob nearest = null;
        float minDist = float.MaxValue;
        
        foreach (var enemy in _enemiesInRange)
        {
            if (!enemy.IsAlive)
            {
                continue;
            }
            float dist = GlobalPosition.DistanceTo(enemy.GlobalPosition);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }
        
        return nearest;
    }
    
    private bool IsEnemy(Mob other)
    {
        return Faction != other.Faction;
    }
    
    // ==================== ПУБЛИЧНЫЕ МЕТОДЫ ====================
    public bool IsAlive => _currentHealth > 0 && _currentState != MobState.Death;
    
    public void TakeDamage(int amount, Mob source = null)
    {
        
        if (_currentState == MobState.Death)
        {
            return;
        }
        
        _currentHealth -= amount;
        _healthBar.SetHealth(_currentHealth, MaxHealth);
        
        // Если получил урон и не в бою - реагируем на обидчика
        if (source != null && IsEnemy(source) && _currentState != MobState.Attack && _currentState != MobState.Chase)
        {
            _currentTarget = source;
            _currentState = MobState.Chase;
        }
        
        if (_currentHealth <= 0)
            Die();
    }
    
    public void MoveTo(Vector2 target)
    {
        _navAgent.TargetPosition = target;
        _currentState = MobState.Move;
    }
    
    private void Die()
    {
        _currentState = MobState.Death;
        _collisionShape.Disabled = true;
        EmitSignal(SignalName.Death);
        
        Tween tween = CreateTween();
        tween.TweenProperty(_sprite, "modulate:a", 0f, 0.4f);
        tween.TweenCallback(Callable.From(() => {
            QueueFree();
        }));
    }
    
    // ==================== ВЛОЖЕННЫЕ ТИПЫ ====================
    private enum MobState
    {
        Idle,
        Move,
        Chase,
        Attack,
        Death
    }
}