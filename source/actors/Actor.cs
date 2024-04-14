using Godot;
using System.Threading.Tasks;
using KidoUtils;
using Game.ActorStatuses;
using Game.Players;
using Game.Damage;
using System;
using System.ComponentModel;

namespace Game.Actors;

public abstract partial class Actor : CharacterBody2D {
	[Export]
	public EffectInflictable Effect {get; protected set;}
	[Export]
	public Damageable DamageableComponent {get; protected set;}
    [Export]
    public AnimatedSprite2D sprite {get; protected set;}
    [Export]
    public CollisionShape2D CollisionShape {get; protected set;}
	
    public ActorStatsManager StatsManager {get; private set;}
    
    [Export]
    protected int moveSpeed;
    protected ModifiedStat MoveSpeed {get; private set;} = new(1, 0);
    public float EffectiveSpeed => MoveSpeed.GetEffectiveValue(moveSpeed);
    
    public ModifiedStat DamageDealt {get; private set;} = new();

	public override void _Process(double delta) => MoveAndSlide();

	public override void _Ready() {
        // safety check for reasons
		ErrorUtils.AvoidEmptyCollisionLayers(DamageableComponent);

        DamageableComponent.OnDamaged += DamageFlash;
        StatsManager = new(SetStats);
        Effect.Init(this);
	}

    protected virtual void SetStats(ActorStats newStats) {
        MoveSpeed = newStats.speed;
        DamageableComponent.damageTaken = newStats.damageTaken;
        DamageDealt = newStats.damageDealt;
        DamageableComponent.maxHealth = newStats.maxHealth;
        DamageableComponent.regenSpeed = newStats.regenSpeed;
    }

	#region Methods

    volatile int percentRed = 0;    
    //Set the flashing to true.
    //If another damage comes in, stop the other flashing and start a new flashing.    
    Color increaseByColor = new(0, 0.05f, 0.05f, 0);

    private async void DamageFlash(DamageInstance _) {
        if (!DamageableComponent.IsAlive) 
            return;

        // expidite the coloring so that we can re-colour again
        if (percentRed != 0) Modulate += increaseByColor * percentRed / 5;
        
        // Reset right as hurt again
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

    public void Flip(bool flip) => sprite.FlipH = flip;

    //Checks to see if this actor can see a player with a ___ pixel width to make sure things like
    //bullets will have enough room to be shot without colliding into a wall for instance. 

    uint raycastCollisionMask = (uint) Layers.Player + (uint) Layers.Environment;
    /// <Summary>
    /// Gap (in pixels) from the center of the actor
    /// </Summary>
    int radius = 10;
	public Player VisiblePlayer() {
        Player visiblePlayer = null;

        // This has a focus on detecting the players damageable component, not their movement hitbox!
        foreach (Player player in Player.Players) {
            if (player is null) continue;
            
            for (int x = -1; x <= 1; x += 2) {
                for (int y = -1; y <= 1; y += 2) {
                    Vector2 from = GlobalPosition;
                    Vector2 to = player.GlobalPosition;

                    var rayQuery = PhysicsRayQueryParameters2D.Create(
                        from, to, raycastCollisionMask, new Godot.Collections.Array<Rid> {GetRid()}
                    );

                    var result = GetWorld2D().DirectSpaceState.IntersectRay(rayQuery);

                    // if (result.Count > 0) GD.Print(result["collider"]);
                    
                    //If the only thing between the player and the enemy is just that -- the enemy and player -- then we good.
                    if (result.Count > 0 && (Rid) result["collider"] == player.GetRid()) visiblePlayer = player;
                    else {
                        // GD.Print("failed at ", x, y);
                        return null;
                    }
                }
            }
        }
        return visiblePlayer;
    }
    
    #endregion
}