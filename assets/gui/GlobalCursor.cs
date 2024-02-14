using Godot;
using System.Collections.Generic;

public partial class GlobalCursor : Area2D {
    public List<Node2D> ObjectsInCursorRange {get; private set;} = new();
    public override void _Process(double delta) {
        Position = GetGlobalMousePosition();
    }

    public void OnBodyEntered(Node2D node) {
        ObjectsInCursorRange.Add(node);
    }
    public void OnBodyExited(Node2D node) {
        ObjectsInCursorRange.Remove(node);
    }
}
