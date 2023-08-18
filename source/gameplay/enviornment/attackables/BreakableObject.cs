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
    private void Destroy(DamageInstance damageInstance) {
        // temp
        QueueFree();
    }

    private void Flash(DamageInstance damageInstance) {
        
    }
}