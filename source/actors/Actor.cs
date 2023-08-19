using Godot;
using System.Threading.Tasks;
using KidoUtils;
using Game.ActorStatuses;
using Game.Players;
using Game.Damage;

namespace Game.Actors;

public abstract partial class Actor : CharacterBody2D {
	
	[Export]
	public EffectInflictable Effect {get; private set;}
	[Export]
	public Damageable DamageableComponent {get; protected set;}
    [Export]
    public AnimatedSprite2D flippedSprite {get; private set;}
    [Export]
    public CollisionShape2D CollisionShape {get; private set;}
	
    
    [Export]
    // Stats
    protected int moveSpeed;
    private float moveSpeedMultiplier = 1f;
	public int EffectiveSpeed {get; protected set;} 

    public float DamageDealingMultplier {get; set;} = 1;


	public override void _Process(double delta) => MoveAndSlide();

	public override void _Ready() {
        // safety check for reasons
		ErrorUtils.AvoidEmptyCollisionLayers(DamageableComponent);

        DamageableComponent.OnDamaged += DamageFlash;
        
        Effect.Init(this);
	}

    public abstract void ApplyStats(ActorStats stats);

	#region Methods

    volatile int percentRed = 0;    
    //Set the flashing to true.
    //If another damage comes in, stop the other flashing and start a new flashing.    
    Color increaseByColor = new(0, 0.05f, 0.05f, 0);

    private async void DamageFlash(DamageInstance _) {
        
        if (!DamageableComponent.IsAlive) return;

        if (percentRed != 0) {
            percentRed = 100;
            return;
        }
        percentRed = 100;
        Modulate -= new Color(0, 1f, 1f, 0);

        while (percentRed > 0) {

            Modulate += increaseByColor;
            await Task.Delay(3);

            percentRed -= 5;
        }
    }

    Vector2 previousFrame;
    public bool IsStalling() {
        Vector2 currentFrame = GlobalPosition;

        float distanceTo = previousFrame.DistanceTo(currentFrame);
        if (distanceTo < 0.01) return true;
        else previousFrame = currentFrame;

        return false;
    }

    public void Flip(bool flip) => flippedSprite.FlipH = flip;

    //Checks to see if this actor can see a player with a ___ pixel width to make sure things like
    //bullets will have enough room to be shot without colliding into a wall for instance. 

    uint raycastCollisionMask = (uint) Layers.Player + (uint) Layers.Environment;
    /// <Summary>
    /// Gap (in pixels) from the center of the actor
    /// </Summary>
	public Player VisiblePlayer(int gap = 0) {
        
        /* Struggling to figure out this gap thing.
        int[] raycastPoints = new int[] {gap/2, -gap/2};
        
        foreach (int )
        */

        foreach (Player player in Player.Players) {
            if (player is null) continue;
            

            var rayQuery = PhysicsRayQueryParameters2D.Create(
                GlobalPosition, player.GlobalPosition, raycastCollisionMask, new Godot.Collections.Array<Rid> {GetRid()}
            );

            var result = GetWorld2D().DirectSpaceState.IntersectRay(rayQuery);

            //If the only thing between the player and the enemy is just that -- the enemy and player -- then we good.
            if (result.Count > 0 && (Rid) result["collider"] == player.GetRid())
                return player;
        }

        return null;
    }
    
    #endregion
}