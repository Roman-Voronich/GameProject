using Godot;

public interface ISkillAction
{
    /// <summary>Проверяет, можно ли применить скилл (ресурсы, дистанция, террейн, юниты)</summary>
    bool CanCast(SkillData skill, Vector2 worldPos, Player player);
    
    /// <summary>Выполняет логику (спавн, урон, бафф и т.д.)</summary>
    void Cast(SkillData skill, Vector2 worldPos, Player player);
}