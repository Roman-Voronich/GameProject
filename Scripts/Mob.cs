using Godot;
using System;
using System.Diagnostics;

namespace GameProject;
[GlobalClass]
public abstract partial class Mob: CharacterBody2D, IEntity, IAttacker, IMovable
{
    // ---- Экспортируемые поля (они же реализуют интерфейсы) ----
    [Export] public string Name { get; set; } = "Mob";
    [Export] public EntityFaction Faction { get; set; } = EntityFaction.Enemy;
    [Export] public float MaxHealth { get; set; } = 100f;
    [Export] public float Speed { get; set; } = 200f;
    [Export] public int Damage { get; set; } = 20;
    [Export] public float AttackRange { get; set; } = 1000f;
    [Export] public float AttackCooldown { get; set; } = 1f;
    [Export] public float DetectionRange { get; set; } = 300f;
    [Export] public int GoldReward { get; set; } = 50;
    
    // ---- Приватные поля ----
    private float _currentHealth;
    private bool _isInvulnerable = false;
    private bool _isSelected = false;
    private bool _isMoving = false;
    private bool _isAttacking = false;
    private float _attackTimer = 0f;
    private IEntity _currentTarget;
    private MobState _currentState = MobState.Idle;
    
    // ---- Компоненты ----
    private TextureProgressBar _healthBar;
    private NavigationAgent2D _navigationAgent;
    private Area2D _detectionArea;
    private CollisionShape2D _collisionShape;
    public event Action<IEntity> OnDeath;
    public event Action<IEntity, float, float> OnHealthChanged;
    public event Action<IAttacker, IEntity> OnAttack;
    // ---- Реализация IEntity ----
    public EntityType Type => EntityType.Mob;
    //public new Vector2 GlobalPosition => base.GlobalPosition;
    public Node2D EntityNode => this;
    public float CurrentHealth => _currentHealth;
    public bool IsAlive => _currentHealth > 0;
    public bool IsEnemy(IEntity other) => Faction != other.Faction;
    public bool IsAlly(IEntity other) => Faction == other.Faction;
    public bool IsAttacking => _isAttacking;

    public void Attack(IEntity target)
    {
        _currentTarget = target;
        _isAttacking = true;
        _currentState = MobState.Attack;
        OnAttack?.Invoke(this, target);
    }

    public bool CanAttackTarget(IEntity target)
    {
        if (target == null || !target.IsAlive || !IsEnemy(target)) return false;
        return GlobalPosition.DistanceTo(target.GlobalPosition) <= AttackRange;
    }
    
    public bool IsMoving => _isMoving;

    public void MoveTo(Vector2 target)
    {
        _navigationAgent.TargetPosition = target;
        _currentState = MobState.Move;
        _isMoving = true;
    }

    public void StopMoving()
    {
        Velocity = Vector2.Zero;
        _currentState = MobState.Idle;
        _isMoving = false;
    }

    public void TakeDamage(float amount, IEntity source = null)
    {
        if (!IsAlive) return;
        float oldHealth = _currentHealth;
        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        OnHealthChanged?.Invoke(this, oldHealth, _currentHealth);
        if (!IsAlive) Die();
    }

    public void Heal(float amount)
    {
        if (!IsAlive) return;
        float oldHealth = _currentHealth;
        _currentHealth = Mathf.Min(MaxHealth, _currentHealth + amount);
        OnHealthChanged?.Invoke(this, oldHealth, _currentHealth);
    }

    private void Die()
    {
        _currentState = MobState.Dead;
        _isAttacking = false;
        _isMoving = false;
        OnDeath?.Invoke(this);
        
        if (_collisionShape != null)
            _collisionShape.Disabled = true;
        
        GetTree().CreateTimer(1f).Timeout+= () => QueueFree();
    }

    public override void _Ready()
    {
        _currentHealth = MaxHealth;
        _navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        _detectionArea = GetNode<Area2D>("DetectionArea");
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        _detectionArea.BodyEntered += body =>
        {
            if (body is IEntity e && IsEnemy(e) && _currentTarget == null)
                _currentTarget = e;
        };
        _detectionArea.BodyExited += body =>
        {
            if (body == _currentTarget) _currentTarget = null;
        };
    }

    private void ProcessIdle() 
    {
            if (_currentTarget != null)
            {
                _currentState = MobState.Chase;
            }
    }

    private void ProcessMove()
    {
        if (_currentTarget != null)
        {
            _currentState = MobState.Chase;
        }
        else if (_navigationAgent.IsNavigationFinished())
        {
            StopMoving();
        }
    }

    private void ProcessChase()
    {
        if (_currentTarget == null || !_currentTarget.IsAlive)
        {
            _currentTarget = null;
            StopMoving();
            return;
        }
        
        MoveTo(_currentTarget.GlobalPosition);
        if (GlobalPosition.DistanceTo(_currentTarget.GlobalPosition) <= AttackRange)
        {
            _isAttacking = true;
            StopMoving();
            _currentState = MobState.Attack;
        }
    }

    private void ProcessAttack(double delta)
    {
        if (_currentTarget == null || !_currentTarget.IsAlive)
        {
            _currentTarget = null;
            _currentState = MobState.Idle;
            _isAttacking = false;
            return;
        }

        if (GlobalPosition.DistanceTo(_currentTarget.GlobalPosition) > AttackRange + 10f)
        {
            _currentState = MobState.Chase;
            _isAttacking = false;
            return;
        }
        _attackTimer +=(float)delta;
        if (_attackTimer >= AttackCooldown)
        {
            _attackTimer = 0f;
            _currentTarget.TakeDamage(Damage,this);
        }
    }

    private void ProcessMovement()
    {
        if (_currentState == MobState.Attack || _currentState == MobState.Dead)
            return;

        if (_navigationAgent.IsNavigationFinished())
            return;

        Vector2 nextPos = _navigationAgent.GetNextPathPosition();
        Vector2 direction = (nextPos - GlobalPosition).Normalized();
        Velocity = direction * Speed;
        MoveAndSlide();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_currentState == MobState.Dead) return;

        if (_currentState != MobState.Attack && _currentTarget != null &&
            (!_currentTarget.IsAlive || !IsEnemy(_currentTarget)))
        {
            _currentTarget = null;
        }

        switch (_currentState)
        {
            case MobState.Chase: ProcessChase(); break;
            case MobState.Attack: ProcessAttack(delta); break;
            case MobState.Move: ProcessMove(); break;
            case MobState.Idle: ProcessIdle(); break;
        }

        ProcessMovement();
    }
    private enum MobState {Idle, Move, Chase, Attack, Dead}
}