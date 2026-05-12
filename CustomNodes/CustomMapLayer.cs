using Godot;
using System;

[Tool, GlobalClass]
public partial class CustomMapLayer : TileMapLayer
{
    public Vector2I GetTilePos() =>
        LocalToMap(ToLocal(GetGlobalMousePosition()));
}