using Godot;
using System;

public partial class FallingArea : Area2D {

    public override void _Ready() {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body) {
        if (body is Actor actor) {
            actor.DamageableComponent.Damage(new(actor) {damage = 1000, forceDirection = Vector2.Down});
            // TODO:
            // The death state takes away from the changes to velocity. We can't have that!
        }
    }
}

