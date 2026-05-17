using System;
using Godot;
namespace GameProject;
public interface IAttacker
{
    int Damage { get; }
    float AttackRange { get; }
    float AttackCooldown { get; }
    bool IsAttacking { get; }
    
    void Attack(IEntity target);
    bool CanAttackTarget(IEntity target);
    
    event Action<IAttacker, IEntity> OnAttack;
}