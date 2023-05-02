using Godot;
using System.Threading.Tasks;
using KidoUtils;

public abstract partial class Actor : CharacterBody2D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public Damageable DamageableComponent {get; private set;}
	
	[Export]
	public int MoveSpeed {get; private set;}
	
	public override void _Process(double delta) {
		MoveAndSlide();
	}

	public override void _Ready() {
		//A safety check for reasons
		ErrorUtils.AvoidEmptyCollisionLayers(DamageableComponent);

		DamageableComponent.OnDeath += OnDeath;
		DamageableComponent.OnDamaged += OnDamaged;
        DamageableComponent.OnDamaged += DamageFlash;

	}
	public abstract void OnDeath(DamageInstance damageInstance);
	public abstract void OnDamaged(DamageInstance damageInstance);


	#region Methods

    private async void DamageFlash(DamageInstance _) {
        Color color = new(1,1,1);
        for (int i = 100; i > 0; i--) {
            color.G = 1 - i / 100f;
            color.B = 1 - i / 100f;
            Modulate = color;
            await Task.Delay(3);
        }
    }

	Vector2 previousVelocity;
    public bool IsStalling(double delta, float timeStalled, ref float stallingTimer) {
        if (previousVelocity == Velocity) {
            
            stallingTimer += (float) delta;

            if (stallingTimer > timeStalled) {
                return true;
            }
        }
        else{
            stallingTimer = 0;
        }

        previousVelocity = Velocity;
        return false;
    }
	
    uint raycastCollisionMask = (uint) Layers.Player + (uint) Layers.Enviornment;
	public Player VisiblePlayer() {
        //Not multiplayer safe (as is many things. This is most likely gonna be single player anyway.)

        //This is the most beutiful line to ever be written.
        foreach (Player player in Player.players) {
            if (player is null) continue;
            
            var spaceState = GetWorld2D().DirectSpaceState;

            var rayQuery = PhysicsRayQueryParameters2D.Create(
                GlobalPosition, player.GlobalPosition, raycastCollisionMask, new Godot.Collections.Array<Rid> { GetRid() }
            );

            var result = spaceState.IntersectRay(rayQuery);

            //If the only thing between the player and the enemy is just that -- the enemy and player -- then we good.
            if (result.Count > 0 && (Rid) result["collider"] == player.GetRid())
                return player;
        }

        return null;
    }


	#endregion
}