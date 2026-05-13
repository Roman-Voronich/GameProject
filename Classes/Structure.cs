using Godot;

public class Structure
{
    public Vector2I Size { get; }
    public int Width { get => Size.X; }
    public int Height { get => Size.Y; }
    public int AtlasX { get; }
    public int AtlasY { get; }

    public Structure(int width, int height, int x, int y)
    {
        Size = new Vector2I(width, height);
        AtlasX = x;
        AtlasY = y;
    }
}