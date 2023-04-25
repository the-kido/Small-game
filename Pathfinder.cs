using Godot;
using System;
using System.Collections.Generic;

public partial class Pathfinder : Node2D
{
    [Export]
    private int HoverAtSpawnPointDistance = 0;
    private Vector2 spawnPoint;
    double timeToMove = 0;

    public override void _Ready() {
        spawnPoint = GlobalPosition;
    }
    public override void _Process(double delta) {
        timeToMove += delta;
        if (timeToMove >= 4) {
            GD.Print("shifted");
            Shift();
            timeToMove = 0;
        }
    }
    private void Shift() {
        Random rand = new((int)Time.GetTicksUsec());

        Vector2 shiftTo = Vector2.One;
        shiftTo.Y *= (float) (rand.NextDouble() - 0.5f) * HoverAtSpawnPointDistance * 2;
        shiftTo.X += (float) (rand.NextDouble() - 0.5f) * HoverAtSpawnPointDistance * 2;
        shiftTo += spawnPoint;
        GD.Print(shiftTo);

        RayCast2D a = new();
        var space = GetWorld2D().DirectSpaceState;
        var query = PhysicsRayQueryParameters2D.Create(spawnPoint, shiftTo);
        Godot.Collections.Dictionary what = space.IntersectRay(query);
        
        if (what.Count > 0) {
            GD.Print("that TP will nto work.");
            Shift();
            return;
        }

        GetParent<Node2D>().Position = shiftTo;
    }
}
