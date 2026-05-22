using Godot;

namespace GameProject.Scripts.Skills;

public class DestroyAction : ISkillAction
{
    public bool CanCast(SkillData skill, Vector2 worldPos, Player player)
    {
        return true;
    }

    public void Cast(SkillData skill, Vector2 worldPos, Player Player)
    {
        if (Player.Mode != PlayerMode.Destroy) Player.Mode = PlayerMode.Destroy;
        else Player.Mode = PlayerMode.Nothing;
    }
}