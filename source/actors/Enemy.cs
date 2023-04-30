using Godot;
using System;
using System.Threading.Tasks;
using KidoUtils;

public partial class Enemy : Actor
{

    //Used for fade in-out.
    [Export]
    private Sprite2D sprite;

    public StateMachine stateMachine = new();

    public override void _Ready() {
        base._Ready();
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public Player VisiblePlayer() {
        //Not multiplayer safe (as is many things. This is most likely gonna be single player anyway.)

        //This is the most beutiful line to ever be written.
        foreach (Player player in Player.players) {
            if (player is null) continue;
            
            uint collisionMask = (uint) Layers.Player + (uint) Layers.Enviornment;

            var spaceState = GetWorld2D().DirectSpaceState;

            var rayQuery = PhysicsRayQueryParameters2D.Create(
                GlobalPosition, player.GlobalPosition, collisionMask, new Godot.Collections.Array<Rid> { GetRid() }
            );

            var result = spaceState.IntersectRay(rayQuery);

            //If the only thing between the player and the enemy is just that -- the enemy and player -- then we good.
            if (result.Count > 0 && (Rid) result["collider"] == player.GetRid())
                return player;
        }

        return null;
    }

    public async override void OnDeath(DamageInstance damageInstance) {
        
        //Temporary implementation jsut to make sure that this works the way I think it would yknow.

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
