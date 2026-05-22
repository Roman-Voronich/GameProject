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
        var ui = player.GetParent().GetNode("UI/Hud");
        var shopNode = ui.GetNode<Shop>("Shop");
        if(shopNode == null && Player.Mode == PlayerMode.Nothing)
        {
            var shop = _shop.Instantiate<Shop>();
            ui.AddChild(shop);
        }
        if(shopNode != null) shopNode.ExitButtonPressed();
        if (Player.Mode == PlayerMode.Build) Player.Mode = PlayerMode.Nothing;
    }
}
