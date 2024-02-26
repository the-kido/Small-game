using Godot;
using Game.Damage;

namespace Game.LevelContent;

public abstract partial class BreakableObject : StaticBody2D {
    [Export]
    protected Damageable Damageable {get; private set;}
    public override void _Ready() {
        Damageable.OnDamaged += Flash;
        Damageable.OnDeath += Destroy;
    }

    protected void Destroy(DamageInstance damageInstance) {
        // temp
        Modulate = new(1,1,1,0.5f);
        // QueueFree();
    }

    private void Flash(DamageInstance damageInstance) {
        
    }
}