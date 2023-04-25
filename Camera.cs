using Godot;
using System;

public partial class Camera : Camera2D
{
    [Export]
    private Player player;
    [Export]
    private TileMap tileMap;

    public override void _Ready()
    {
        
        Vector2I b = tileMap.TileSet.TileSize;
        Rect2 a = tileMap.GetUsedRect();
        LimitLeft = (int) (a.Position.X * b.X * tileMap.Scale.X);
        LimitTop = (int) (a.Position.Y * b.Y * tileMap.Scale.Y);
        LimitRight = (int) (a.End.X * b.X * tileMap.Scale.X);
        LimitBottom = (int) (a.End.Y * b.Y * tileMap.Scale.Y);

    }
    public override void _Process(double delta) {
        
        Position = player.Position;
    }

}
