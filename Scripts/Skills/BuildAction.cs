using Godot;

namespace GameProject.Scripts.Skills;

public class BuildAction : ISkillAction
{
    public bool CanCast(SkillData skill, Vector2 worldPos, Player player)
    {
        return true;
    }

    public void Cast(SkillData skill, Vector2 worldPos, Player player)
    {
        GD.Print("Build action cast");
        player.ChangeMode();
    }
}