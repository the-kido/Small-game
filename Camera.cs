using Godot;
using System;

public partial class Camera : Camera2D
{
    [Export]
    private Player player;
    [Export]
    private TileMap tileMap;
    private MovementController movementController;

    public override void _Ready() {
        movementController = player.GetNode<MovementController>("Movement Controller");
        
        #region initialize the size of the camera for this level
        Vector2I ts = tileMap.TileSet.TileSize;
        Rect2 rect = tileMap.GetUsedRect();

        LimitLeft = (int) (rect.Position.X * ts.X * tileMap.Scale.X);
        LimitTop = (int) (rect.Position.Y * ts.Y * tileMap.Scale.Y);
        LimitRight = (int) (rect.End.X * ts.X * tileMap.Scale.X);
        LimitBottom = (int) (rect.End.Y * ts.Y * tileMap.Scale.Y);
        #endregion

    }
    public override void _Process(double delta) {
        
        Vector2 cameraShift = movementController.playerDirection * 100;
        cameraShift.Y *= 1.5f;

        Vector2 gotoPosition = player.Position + cameraShift;
        
        Position = Position.Lerp(gotoPosition, (float) delta * 2);
        
    }
}
