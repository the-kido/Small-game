using Godot;
using System;
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
	}
	public abstract void OnDeath(DamageInstance damageInstance);
	public abstract void OnDamaged(DamageInstance damageInstance);


	#region Methods
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
	#endregion


}


