using Godot;
public partial class FallingArea : Area2D {

    public override void _Ready() => 
        BodyEntered += OnBodyEntered;
    
    private void OnBodyEntered(Node2D body) {
        if (body is Enemy actor)
            actor.DamageableComponent.Damage(new(actor, DamageInstance.Type.Void) {damage = 1000});
    }
}

