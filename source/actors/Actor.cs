using Godot;
using System;

public abstract partial class Actor : CharacterBody2D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public Damageable DamageableComponent {get; private set;}

	public override void _Ready() {
		DamageableComponent.OnDeath += OnDeath;
		DamageableComponent.OnDamaged += OnDamaged;
	}
	public abstract void OnDeath(DamageInstance damageInstance);
	public abstract void OnDamaged(DamageInstance damageInstance);
}
