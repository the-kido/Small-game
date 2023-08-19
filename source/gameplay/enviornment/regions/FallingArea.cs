using Godot;
using Game.Actors;
using Game.Damage;

namespace Game.LevelContent;

public partial class FallingArea : Area2D {

    public override void _Ready() => 
        BodyEntered += OnBodyEntered;
    
    private void OnBodyEntered(Node2D body) {
        if (body is Enemy actor)
            actor.DamageableComponent.Kill(new(actor, DamageInstance.Type.Void));
    }
}

