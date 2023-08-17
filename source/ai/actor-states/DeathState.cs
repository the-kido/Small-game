using Godot;
using System.Threading.Tasks;
using Game.Damage;

namespace Game.Actors.AI;

public sealed class FallDeathState : AIState {
    public override async void Init() {
        await Task.Delay(200);
        actor.ZIndex = -3;
    }

    public override void Update(double delta) {
        actor.Velocity += Vector2.Down * 500 * (float) delta; 
        actor.Scale -= Vector2.One * (float) delta / 1.3f;
        actor.Rotate(6 * (float) delta);
    }
}

public sealed class DeathState : AIState {
    DamageInstance damageInstance;
    public DeathState(DamageInstance damageInstance) {
        this.damageInstance = damageInstance; 
    }
    
    public async override void Init() {
        //Temporary implementation jsut to make sure that this works the way I think it would yknow.
        //in reality, i shoulda put all of this in update and used Delta but idk what I was thinking before.
        int deathSpeed = 1;
        
        Vector2 start = damageInstance.forceDirection * 100;
        actor.Velocity = start;

        Color color = new Color(1,1,1,1);
        
        for (float i = 10; i < 100; i+= deathSpeed) {
            color.A = 1 - i / 100;
            actor.Modulate = color;

            float math = Mathf.Log(i/100) + 1;
            actor.Velocity = start.Lerp(Vector2.Zero, math);

            await Task.Delay(10);
        }
        actor.QueueFree();
    }

    public override void Update(double delta) {}
}