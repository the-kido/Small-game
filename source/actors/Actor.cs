using Godot;
using System.Threading.Tasks;
using KidoUtils;

public abstract partial class Actor : CharacterBody2D {
	
	[Export]
	public EffectInflictable Effect {get; private set;}
	[Export]
	public Damageable DamageableComponent {get; private set;}
    [Export]
    public AnimatedSprite2D flippedSprite {get; private set;}
    [Export]
    public CollisionShape2D CollisionShape {get; private set;}
	[Export]

    // Stats
	public int MoveSpeed {get; set;}
    public float DamageDealingMultplier {get; set;} = 1;

	public override void _Process(double delta) => MoveAndSlide();

	public override void _Ready() {
        // safety check for reasons
		ErrorUtils.AvoidEmptyCollisionLayers(DamageableComponent);

        DamageableComponent.OnDamaged += DamageFlash;
        DamageableComponent.OnDamaged += (damageInstance) => Effect.Add(damageInstance.statusEffect);

        Effect.Init(this);
	}

	#region Methods

    volatile int percentRed = 0;    
    //Set the flashing to true.
    //If another damage comes in, stop the other flashing and start a new flashing.    
    private async void DamageFlash(DamageInstance _) {
        if (!DamageableComponent.IsAlive) return;

        if (percentRed != 0) {
            percentRed = 100;
            return;
        }
        percentRed = 100;

        Color color = new(1,1,1);
        while (percentRed > 0) {

            color.G = 1 - percentRed / 100f;
            color.B = 1 - percentRed / 100f;
            Modulate = color;
            await Task.Delay(3);
            percentRed-=5;
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

    uint raycastCollisionMask = (uint) Layers.Player + (uint) Layers.Enviornment;
    /// <Summary>
    /// Gap (in pixels) from the center of the actor
    /// </Summary>
	public Player VisiblePlayer(int gap = 0) {
        
        /* Struggling to figure out this gap thing.
        int[] raycastPoints = new int[] {gap/2, -gap/2};
        
        foreach (int )
        */

        foreach (Player player in Player.players) {
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