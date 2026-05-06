public partial class TileResourceInfo
{
    public string type;
    public int count;
    public int hp;

    public TileResourceInfo(string type, int count, int hp)
    {
        this.type = type;
        this.count = count;
        this.hp = hp;
    }
}
