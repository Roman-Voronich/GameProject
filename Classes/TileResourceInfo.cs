public partial class TileResourceInfo
{
    public ResourceType type;
    public int count;
    public int hp;

    public TileResourceInfo(ResourceType type, int count, int hp)
    {
        this.type = type;
        this.count = count;
        this.hp = hp;
    }
}
