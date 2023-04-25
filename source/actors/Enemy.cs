using Godot;
using System;
using System.Threading.Tasks;

public partial class Enemy : Actor
{

    [Export]
    private Sprite2D sprite; 

    public override void _Ready() {
        base._Ready();
        
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public async override void OnDeath(DamageInstance damageInstance) {
        
        int deathSpeed = 1;


        //No longer recieve damage.
        DamageableComponent.QueueFree();
        CollisionLayer = 0;
        CollisionMask = (int) Layers.Enviornment;
        


        Vector2 start = damageInstance.forceDirection * 100;
        Velocity = start;

        for (float i = 10; i < 100; i+= deathSpeed) {
            float math = Mathf.Log(i/100) + 1;
            Velocity = start.Lerp(Vector2.Zero, Mathf.Log(i/100) + 1);
            await Task.Delay(10);
        }
        
        QueueFree();
    }

    public override void OnDamaged(DamageInstance damageInstance) {
        
    }
}
