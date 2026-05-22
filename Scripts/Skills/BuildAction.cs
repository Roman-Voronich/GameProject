using Godot;

namespace GameProject.Scripts.Skills;

public class BuildAction : ISkillAction
{
    public bool CanCast(SkillData skill, Vector2 worldPos, Player player)
    {
        return true;
    }

    public void Cast(SkillData skill, Vector2 worldPos, Player Player)
    {
        if (Player.Mode != PlayerMode.Build) Player.Mode = PlayerMode.Build;
        else Player.Mode = PlayerMode.Nothing;
    }
}