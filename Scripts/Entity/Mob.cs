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
        // ДИАГНОСТИКА
        GD.Print($"=== ДИАГНОСТИКА {Name} ===");
        GD.Print($"Position: {GlobalPosition}");
    
        // Проверяем наличие узлов
        GD.Print($"Has NavigationAgent2D: {HasNode("NavigationAgent2D")}");
        GD.Print($"Has DetectionArea: {HasNode("DetectionArea")}");
        GD.Print($"Has CollisionShape2D: {HasNode("CollisionShape2D")}");
        GD.Print($"Has HealthBar: {HasNode("HealthBar")}");
        GD.Print($"Has Sprite2D: {HasNode("Sprite2D")}");
        _currentHealth = MaxHealth;
        
        _navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        _detectionArea = GetNode<Area2D>("DetectionArea");
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        _healthBar = GetNode<HealthBar>("HealthBar");
        _sprite = GetNode<Sprite2D>("Sprite2D");
        
        _healthBar.SetHealth(_currentHealth, MaxHealth);
        
        // ДЕБАГ: сообщение о готовности
        GD.Print($"[{Name}] Готов! Фракция: {Faction}, HP: {_currentHealth}, DetectionRange: {DetectionRange}");
        
        // Настройка зоны обнаружения
        _detectionArea.BodyEntered += OnBodyEntered;
        _detectionArea.BodyExited += OnBodyExited;
        
        if (_detectionArea.GetNode<CollisionShape2D>("CollisionShape2D")?.Shape is CircleShape2D circle)
        {
            circle.Radius = DetectionRange;
            GD.Print($"[{Name}] Радиус детекции установлен: {circle.Radius}");
        }
        else
        {
            GD.PrintErr($"[{Name}] НЕ НАЙДЕН CircleShape2D в DetectionArea!");
        }
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (_currentState == MobState.Death) return;
        
        // ДЕБАГ: текущее состояние (раз в 2 секунды, чтобы не заспамить)
        if (Time.GetTicksMsec() % 120 == 0)
        {
            GD.Print($"[{Name}] Состояние: {_currentState}, Врагов в зоне: {_enemiesInRange.Count}, Текущая цель: {(_currentTarget != null ? _currentTarget.Name : "null")}");
        }
        
        // Обновляем список живых врагов
        int beforeCount = _enemiesInRange.Count;
        _enemiesInRange.RemoveAll(e => e == null || !e.IsAlive);
        if (beforeCount != _enemiesInRange.Count)
        {
            GD.Print($"[{Name}] Обновлён список врагов: {beforeCount} -> {_enemiesInRange.Count}");
        }
        
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
        GD.Print($"[{Name}] ProcessIdle: врагов в зоне {_enemiesInRange.Count}");
        
        if (_enemiesInRange.Count > 0)
        {
            _currentTarget = FindNearestEnemy();
            GD.Print($"[{Name}] Нашёл цель в Idle: {(_currentTarget != null ? _currentTarget.Name : "null")}");
            if (_currentTarget != null)
                _currentState = MobState.Chase;
        }
    }
    
    private void ProcessChase()
    {
        if (_currentTarget == null || !_currentTarget.IsAlive)
        {
            GD.Print($"[{Name}] ProcessChase: цель потеряна или мертва. Ищу новую...");
            _currentTarget = FindNearestEnemy();
            if (_currentTarget == null)
            {
                GD.Print($"[{Name}] ProcessChase: новых целей нет, ухожу в Idle");
                _currentState = MobState.Idle;
                return;
            }
            GD.Print($"[{Name}] ProcessChase: новая цель {_currentTarget.Name}");
        }
        
        float distance = GlobalPosition.DistanceTo(_currentTarget.GlobalPosition);
        GD.Print($"[{Name}] ProcessChase: дистанция до {_currentTarget.Name} = {distance:F1} (AttackRange: {AttackRange})");
        
        if (distance <= AttackRange)
        {
            GD.Print($"[{Name}] ProcessChase: цель в зоне атаки! Перехожу в Attack");
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
            GD.Print($"[{Name}] ProcessAttack: цель {(_currentTarget != null ? _currentTarget.Name : "null")} мертва или null. Ищу новую...");
            _currentTarget = FindNearestEnemy();
            if (_currentTarget == null)
            {
                GD.Print($"[{Name}] ProcessAttack: новых целей нет, ухожу в Idle");
                _currentState = MobState.Idle;
                return;
            }
            GD.Print($"[{Name}] ProcessAttack: новая цель {_currentTarget.Name}, перехожу в Chase");
            _currentState = MobState.Chase;
            return;
        }
        
        float distance = GlobalPosition.DistanceTo(_currentTarget.GlobalPosition);
        GD.Print($"[{Name}] ProcessAttack: дистанция до {_currentTarget.Name} = {distance:F1}");
        
        if (distance > AttackRange + 20f)
        {
            GD.Print($"[{Name}] ProcessAttack: цель убежала, перехожу в Chase");
            _currentState = MobState.Chase;
            return;
        }
        
        _attackTimer += (float)delta;
        if (_attackTimer >= AttackCooldown)
        {
            _attackTimer = 0f;
            GD.Print($"[{Name}] АТАКУЕТ {_currentTarget.Name} на {Damage} урона!");
            _currentTarget.TakeDamage(Damage, this);
            
            // Если цель умерла - сразу ищем нового
            if (!_currentTarget.IsAlive)
            {
                GD.Print($"[{Name}] ProcessAttack: цель {_currentTarget.Name} умерла! Ищу нового...");
                _currentTarget = FindNearestEnemy();
                if (_currentTarget == null)
                {
                    GD.Print($"[{Name}] ProcessAttack: новых целей нет, ухожу в Idle");
                    _currentState = MobState.Idle;
                }
                else
                {
                    GD.Print($"[{Name}] ProcessAttack: новая цель {_currentTarget.Name}, продолжаю бой");
                    _currentState = MobState.Chase;
                }
            }
        }
    }
    
    private void ProcessMove()
    {
        if (_enemiesInRange.Count > 0)
        {
            GD.Print($"[{Name}] ProcessMove: обнаружен враг! Перехожу в Chase");
            _currentTarget = FindNearestEnemy();
            _currentState = MobState.Chase;
            return;
        }
        
        if (_navAgent.IsNavigationFinished())
        {
            GD.Print($"[{Name}] ProcessMove: навигация завершена, ухожу в Idle");
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
        GD.Print($"[{Name}] OnBodyEntered: {body.Name} вошёл в зону!");
        
        if (body == this)
        {
            GD.Print($"[{Name}] OnBodyEntered: это я сам, игнорирую");
            return;
        }
        
        if (body is Mob mob)
        {
            GD.Print($"[{Name}] OnBodyEntered: {body.Name} это Mob, фракция {mob.Faction}, моя фракция {Faction}");
            
            if (IsEnemy(mob))
            {
                GD.Print($"[{Name}] OnBodyEntered: {body.Name} ВРАГ! Добавляю в список");
                if (mob.IsAlive)
                {
                    _enemiesInRange.Add(mob);
                    GD.Print($"[{Name}] OnBodyEntered: список врагов теперь содержит {_enemiesInRange.Count} элементов");
                }
                else
                {
                    GD.Print($"[{Name}] OnBodyEntered: {body.Name} мёртв, не добавляю");
                }
            }
            else
            {
                GD.Print($"[{Name}] OnBodyEntered: {body.Name} союзник, игнорирую");
            }
        }
        else
        {
            GD.Print($"[{Name}] OnBodyEntered: {body.Name} не является Mob, игнорирую");
        }
    }
    
    private void OnBodyExited(Node2D body)
    {
        GD.Print($"[{Name}] OnBodyExited: {body.Name} вышел из зоны!");
        
        if (body is Mob mob)
        {
            bool wasRemoved = _enemiesInRange.Remove(mob);
            if (wasRemoved)
                GD.Print($"[{Name}] OnBodyExited: {body.Name} удалён из списка врагов. Осталось: {_enemiesInRange.Count}");
            
            if (_currentTarget == mob)
            {
                GD.Print($"[{Name}] OnBodyExited: это была моя цель! Ищу новую...");
                _currentTarget = FindNearestEnemy();
            }
        }
    }
    
    private Mob FindNearestEnemy()
    {
        GD.Print($"[{Name}] FindNearestEnemy: поиск среди {_enemiesInRange.Count} врагов");
        
        Mob nearest = null;
        float minDist = float.MaxValue;
        
        foreach (var enemy in _enemiesInRange)
        {
            if (!enemy.IsAlive)
            {
                GD.Print($"[{Name}] FindNearestEnemy: {enemy.Name} мёртв, пропускаю");
                continue;
            }
            float dist = GlobalPosition.DistanceTo(enemy.GlobalPosition);
            GD.Print($"[{Name}] FindNearestEnemy: {enemy.Name} на дистанции {dist:F1}");
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
                GD.Print($"[{Name}] FindNearestEnemy: {enemy.Name} пока ближайший");
            }
        }
        
        GD.Print($"[{Name}] FindNearestEnemy: результат = {(nearest != null ? nearest.Name : "null")}, дистанция {minDist:F1}");
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
        GD.Print($"[{Name}] TakeDamage: получен урон {amount} от {(source != null ? source.Name : "null")}");
        
        if (_currentState == MobState.Death)
        {
            GD.Print($"[{Name}] TakeDamage: уже мёртв, урон проигнорирован");
            return;
        }
        
        _currentHealth -= amount;
        _healthBar.SetHealth(_currentHealth, MaxHealth);
        GD.Print($"[{Name}] TakeDamage: здоровье стало {_currentHealth}/{MaxHealth}");
        
        // Если получил урон и не в бою - реагируем на обидчика
        if (source != null && IsEnemy(source) && _currentState != MobState.Attack && _currentState != MobState.Chase)
        {
            GD.Print($"[{Name}] TakeDamage: реагирую на атаку от {source.Name}, перехожу в Chase");
            _currentTarget = source;
            _currentState = MobState.Chase;
        }
        
        if (_currentHealth <= 0)
            Die();
    }
    
    public void MoveTo(Vector2 target)
    {
        GD.Print($"[{Name}] MoveTo: двигаюсь к {target}");
        _navAgent.TargetPosition = target;
        _currentState = MobState.Move;
    }
    
    private void Die()
    {
        GD.Print($"[{Name}] Die: умираю!");
        _currentState = MobState.Death;
        _collisionShape.Disabled = true;
        EmitSignal(SignalName.Death);
        
        Tween tween = CreateTween();
        tween.TweenProperty(_sprite, "modulate:a", 0f, 0.4f);
        tween.TweenCallback(Callable.From(() => {
            GD.Print($"[{Name}] Die: полностью удалён");
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