using Godot;
using System;
using KidoUtils;

public abstract partial class Actor : CharacterBody2D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public Damageable DamageableComponent {get; private set;}

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
}
