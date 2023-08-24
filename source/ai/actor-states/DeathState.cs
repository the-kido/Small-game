using Godot;
using System.Threading.Tasks;
using Game.Damage;

namespace Game.Actors.AI;

public sealed class FallDeathState : AIState {
    public override async void Init() {
        await Task.Delay(200);  // This is fine
        actor.ZIndex = -3;
    }

    public override void Update(double delta) {
        actor.Velocity += Vector2.Down * 500 * (float) delta; 
        actor.Scale -= Vector2.One * (float) delta / 1.3f;
        actor.Rotate(6 * (float) delta);
    }
}

public sealed class DeathState : AIState {

    public DeathState(DamageInstance damageInstance) 
        => destination = damageInstance.forceDirection * DISTANCE;
    
    const int DISTANCE = 100;
    KidoUtils.Timer timer = new(time: 0.01, cycles: 90); // This is the first time I've used this feature
    Color color = new(1,1,1,1);
    Vector2 destination;
    
    int i = 10;

    public override void Init() {
        timer.TimeOver += UpdateActorStuff;
        timer.AllLoopsFinished += actor.QueueFree;
    }
    public override void Update(double delta) 
        => timer.Update(delta);

    private void UpdateActorStuff() {
        color.A = 1f - i / 100f;
        actor.Modulate = color;
        
        float math = Mathf.Log(i / 100f) + 1f;
        actor.Velocity = destination.Lerp(Vector2.Zero, math);

        i+=1;
    }
}