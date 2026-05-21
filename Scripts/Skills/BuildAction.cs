using System.Linq;
using Godot;

namespace GameProject.Scripts.Skills;

public class BuildAction : ISkillAction
{
    private PackedScene _shop = GD.Load<PackedScene>("res://Scenes/UI/Shop.tscn");
    public bool CanCast(SkillData skill, Vector2 worldPos, Player player)
    {
        return true;
    }

    public void Cast(SkillData skill, Vector2 worldPos, Player player)
    {
        GD.Print("Build action cast");
        var ui = player.GetParent().GetNode("UI");
        if(player.isBuildMode)player.ChangeMode();
        else
        {
            var shopNode = ui.GetNode<Shop>("Shop");
            if (shopNode != null) shopNode.ExitButtonPressed();
            else
            {
                var shop = _shop.Instantiate<Shop>();
                ui.AddChild(shop);
            }
        }
    }
}
