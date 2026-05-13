using Godot;

namespace GameProject.Scripts.Skills;

public class DestroyAction : ISkillAction
{
    public bool CanCast(SkillData skill, Vector2 worldPos, Player player)
    {
        return true;
    }

    public void Cast(SkillData skill, Vector2 worldPos, Player player)
    {
        if(!player.isBuildMode || player.isRemoveMode) player.ChangeMode();
        player.isRemoveMode = !player.isRemoveMode;
    }
}