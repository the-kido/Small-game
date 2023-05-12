using KidoUtils;
using Godot;
using System.Threading.Tasks;

public sealed class DeathState : AIState
{
    DamageInstance damageInstance;
    public DeathState(DamageInstance damageInstance) {
        this.damageInstance = damageInstance; 
    }
    
    public async override void Init() {

        //Temporary implementation jsut to make sure that this works the way I think it would yknow.

        int deathSpeed = 1;

        //No longer recieve damage.
        actor.DamageableComponent.QueueFree();
        actor.CollisionLayer = 0;
        actor.CollisionMask = (int) Layers.Enviornment;
        
        Vector2 start = damageInstance.forceDirection * 100;
        actor.Velocity = start;

        Color color = new Color(1,1,1,1);

        for (float i = 10; i < 100; i+= deathSpeed) {
            color.A = 1 - i/100;
            actor.Modulate = color;

            float math = Mathf.Log(i/100) + 1;
            actor.Velocity = start.Lerp(Vector2.Zero, Mathf.Log(i/100) + 1);
            await Task.Delay(10);
        }
        actor.QueueFree();
    }

    public override void Update(double delta) {
    }
}