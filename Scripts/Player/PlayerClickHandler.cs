using Godot;

namespace GameProject;

public partial class PlayerClickHandler : Node
{
    [Export] public int AttackDamage = 25;
    [Export] public float AttackCooldown = 1f; // Задержка между ударами в секундах
    
    private bool _canAttack = true;
    private Timer _cooldownTimer;
    
    public override void _Ready()
    {
        // Создаём таймер для кулдауна
        _cooldownTimer = new Timer();
        _cooldownTimer.WaitTime = AttackCooldown;
        _cooldownTimer.OneShot = true;
        _cooldownTimer.Timeout += OnCooldownEnded;
        AddChild(_cooldownTimer);
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && 
            mouseEvent.ButtonIndex == MouseButton.Left && 
            mouseEvent.Pressed && 
            _canAttack) // Проверяем, можем ли атаковать
        {
            Vector2 clickPosition = GetGlobalMousePosition();
            TryAttackMobAtPosition(clickPosition);
        }
    }
    
    private void TryAttackMobAtPosition(Vector2 globalPosition)
    {
        // Получаем World2D через GetTree().Root
        var world2d = GetTree().Root.GetViewport().GetWorld2D();
        var spaceState = world2d.DirectSpaceState;
        
        var parameters = new PhysicsPointQueryParameters2D
        {
            Position = globalPosition,
            CollisionMask = 2,
            CollideWithBodies = true, // Твой Mob это CharacterBody2D (Body)
            CollideWithAreas = true
        };
        
        var results = spaceState.IntersectPoint(parameters);
        results.Reverse();
        foreach (var result in results)
        {
            if (result["collider"].As<GodotObject>() is Mob mob)
            {
                if (mob.Faction == EntityFaction.Enemy) mob.TakeDamage(AttackDamage);
                if (mob.Faction == EntityFaction.Player) mob.Heal(AttackDamage);
                // Запускаем кулдаун после успешной атаки
                StartCooldown();
                break;
            }
        }
    }
    
    private void StartCooldown()
    {
        _canAttack = false;
        _cooldownTimer.Start();
    }
    
    private void OnCooldownEnded()
    {
        _canAttack = true;
        GD.Print("Кулдаун закончился, можно атаковать снова");
    }
    
    private Vector2 GetGlobalMousePosition()
    {
        // Конвертируем экранные координаты в глобальные координаты мира
        var camera = GetViewport().GetCamera2D();
        if (camera != null)
        {
            return camera.GetGlobalMousePosition();
        }
        
        // Fallback: просто возвращаем экранные координаты (может не работать)
        return GetViewport().GetMousePosition();
    }
    
    private Viewport GetViewport()
    {
        return GetTree().Root.GetViewport();
    }
}