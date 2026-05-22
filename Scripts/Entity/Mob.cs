using Godot;
using System;
using System.Collections.Generic;

namespace GameProject;

[GlobalClass]
public abstract partial class Mob : CharacterBody2D, IEntity
{
    // ==================== ЭКСПОРТИРУЕМЫЕ ПОЛЯ ====================
    [Export] public string Name { get; set; } = "Mob";
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
    private IEntity _currentTarget;
    private List<IEntity> _enemiesInRange = new List<IEntity>();
    private IEntity _absoluteTarget;

    private bool _isFirstAttack = true;
    // ==================== КОМПОНЕНТЫ ====================
    private HealthBar _healthBar;
    private NavigationAgent2D _navAgent;
    private Area2D _detectionArea;
    private CollisionPolygon2D _collisionShape;
    private Sprite2D _sprite; 
    private Area2D _clickableArea;
    
    // ==================== СИГНАЛЫ ====================
    [Signal] public delegate void DeathEventHandler();
    
    // ==================== GODOT МЕТОДЫ ====================
    public override void _Ready()
    {
        _currentHealth = MaxHealth;
        var temp = GetNode<Area2D>("ClickArea");
        temp.MouseEntered += Test;
        
        _navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        _detectionArea = GetNode<Area2D>("DetectionArea");
        _collisionShape = GetNode<CollisionPolygon2D>("CollisionPolygon2D");
        _healthBar = GetNode<HealthBar>("HealthBar");
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _clickableArea = GetNode<Area2D>("ClickArea");
        _clickableArea.AddToGroup("clickable");

        _healthBar.Visible = false;
        // Настройка зоны обнаружения
        if (_detectionArea.GetNode<CollisionShape2D>("CollisionShape2D")?.Shape is CircleShape2D circle)
        {
            circle.Radius = DetectionRange;
        }

        _detectionArea.BodyEntered += OnBodyEntered;
        _detectionArea.BodyExited += OnBodyExited;
    }

    public void Test()
    {
        GD.Print("AAAAAAAAAAAAAAAAAA");
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
        _currentTarget = FindNearestEnemy();
        if (_currentTarget != null)
            _currentState = MobState.Chase;
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
        
        if (distance > AttackRange + 2f)
        {
            _currentState = MobState.Chase;
            return;
        }
        
        _attackTimer += (float)delta;
        if (_attackTimer >= AttackCooldown || _isFirstAttack)
        {
            _attackTimer = 0f;
            _currentTarget.TakeDamage(Damage, this);
            _isFirstAttack = false;
            // Если цель умерла - сразу ищем нового
            if (!_currentTarget.IsAlive)
            {
                _isFirstAttack = true;
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
        _currentTarget = FindNearestEnemy();
        if (_currentTarget !=null)
        {
            _navAgent.TargetPosition = _currentTarget.GlobalPosition;
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
        
        if (body is Mob mob && mob.Faction!=this.Faction)
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
    
    private IEntity FindNearestEnemy()
    {
        
        IEntity nearest = null;
        float minDist = float.MaxValue;
        if (_enemiesInRange.Count == 0) return _absoluteTarget; 
        
        foreach (var enemy in _enemiesInRange)
        {
            if (!enemy.IsAlive || enemy == null)
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
    
    public bool IsEnemy(IEntity other)
    {
        return Faction != other.Faction;
    }
    
    // ==================== ПУБЛИЧНЫЕ МЕТОДЫ ====================
    public bool IsAlive => _currentHealth > 0 && _currentState != MobState.Death;
    
    public void TakeDamage(int amount, IEntity source = null)
    {
        GD.Print(_currentHealth);
        if (_currentState == MobState.Death)
        {
            return;
        }
        
        _currentHealth -= amount;
        _healthBar.SetHealth(_currentHealth, MaxHealth);
        GD.Print(_currentHealth);
        // Если получил урон и не в бою - реагируем на обидчика
        if (source is Mob && IsEnemy(source) && _currentState != MobState.Attack && _currentState != MobState.Chase)
        {
            _currentTarget = source;
            _currentState = MobState.Chase;
        }
        
        if (_currentHealth <= 0)
            Die();
    }

    public void Heal(int amount, IEntity source = null)
    {
        if (_currentHealth + amount > MaxHealth || _currentState == MobState.Death) return;
        _currentHealth += amount;
        _healthBar.SetHealth(_currentHealth, MaxHealth);
    }
    
    public void MoveTo(Vector2 target)
    {
        _navAgent.TargetPosition = target;
        _currentState = MobState.Move;
    }

    public void Select()
    {
        _sprite.Texture = GD.Load<Texture2D>("res://Textures/SelectedAlly.png");
    }

    public void Unselect()
    {
        _sprite.Texture = GD.Load<Texture2D>("res://Textures/Ally.png");
    }

    public void SetUpTarget(Mob target)
    {
        _absoluteTarget = target;
    }

    public void SetOffTarget()
    {
        _absoluteTarget = null;
        GD.Print(1);
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