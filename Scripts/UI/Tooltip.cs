using Godot;

public partial class Tooltip : Control
{
    public static Tooltip Instance { get; private set; }
    
    private Label _text;
    private Vector2 _offset = new(15, 15);
    private Vector2 _viewportSize;

    public override void _Ready()
    {
        Instance = this;
        _text = GetNode<Label>("Text");
        MouseFilter = MouseFilterEnum.Ignore; // Чтобы тултип не перехватывал клик
        _viewportSize = GetViewportRect().Size;
    }

    public void Show(string text, Vector2 position)
    {
        if (string.IsNullOrEmpty(text))
        {
            Hide();
            return;
        }

        _text.Text = text;
        Show();
        
        // Позиционируем с учётом границ экрана
        Vector2 pos = position + _offset;
        if (pos.X + Size.X > _viewportSize.X)
            pos.X = position.X - Size.X - _offset.X;
        if (pos.Y + Size.Y > _viewportSize.Y)
            pos.Y = position.Y - Size.Y - _offset.Y;
            
        GlobalPosition = pos;
    }

    public override void _Process(double delta)
    {
        if (Visible)
        {
            Position = GetGlobalMousePosition();
        }
    }

    public void Hide() => Visible = false;
}