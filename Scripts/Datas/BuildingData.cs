using Godot;

[GlobalClass]
public partial class BuildingData : Resource
{
    [Export] public string Name { get; set; } = "Farm";
    [Export] public Texture2D Icon { get; set; }
    [Export] public int TileId { get; set; } = 1;
    [Export] public Vector2I Size { get; set; } = new(1, 1);
    [Export] public int WoodCost { get; set; } = 0;
    [Export] public int StoneCost { get; set; } = 0;
    [Export] public int CopperCost { get; set; } = 0;
    [Export] public int IronCost { get; set; } = 0;
    [Export(PropertyHint.MultilineText)] public string Description { get; set; } = "Производит золото каждые 5 секунд";
}