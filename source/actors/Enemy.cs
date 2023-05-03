using Godot;
using System;
using System.Threading.Tasks;
using KidoUtils;

public partial class Enemy : Actor
{

    //Used for fade in-out.
    [Export]
    private Sprite2D sprite;

    public AIStateMachine StateMachine {get; init;}
    public Enemy() {
        StateMachine = new(this);
    }
    NoState noState = new();
    public override void _Ready() {
        base._Ready();
        StateMachine.AddState(noState, null);
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public async override void OnDeath(DamageInstance damageInstance) {
        StateMachine.ChangeState(noState);

        //Temporary implementation jsut to make sure that this works the way I think it would yknow.

        int deathSpeed = 1;

        //No longer recieve damage.
        DamageableComponent.QueueFree();
        CollisionLayer = 0;
        CollisionMask = (int) Layers.Enviornment;
        
        Vector2 start = damageInstance.forceDirection * 100;
        Velocity = start;

        Color color = new Color(1,1,1,1);

        for (float i = 10; i < 100; i+= deathSpeed) {
            color.A = 1 - i/100;
            Modulate = color;

            float math = Mathf.Log(i/100) + 1;
            Velocity = start.Lerp(Vector2.Zero, Mathf.Log(i/100) + 1);
            await Task.Delay(10);
        }
        QueueFree();
    }

    public override void OnDamaged(DamageInstance damageInstance) {
        
    }
}
