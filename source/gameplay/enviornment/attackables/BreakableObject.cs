using Godot;

public abstract partial class BreakableObject : StaticBody2D {
    [Export]
    protected Damageable Damageable {get; private set;}
    public override void _Ready() {
        GD.Print(Damageable);
        Damageable.OnDamaged += Flash;
        Damageable.OnDeath += Destroy;
    }
    private void Destroy(DamageInstance damageInstance) {
        // temp
        GD.Print("should queuefree");
        QueueFree();
    }

    private void Flash(DamageInstance damageInstance) {
        
    }
}