using Godot;
using System;

public partial class TextureButton : Godot.TextureButton
{
    public override void _Pressed()
    {
        GetParent().RemoveChild(this);
    }
}
