using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Damageable : Area2D
{
	//Do everything a damageable entity would need
	[Export]
	public int Health{get; private set;}
	[Export(PropertyHint.None, "In seconds: how long it takes for this entity to be hit again")]
	private double ImmunityFrames {get; set;}
	
	private bool IsImmune = false;
	private int MaxHealth {get; init;}
	

	public delegate void HealthDepletedEventHandler(DamageInstance damageInstance);
	public event HealthDepletedEventHandler Ondamaged;
	public delegate void DeathEventHandler(DamageInstance damageInstance);
	public event DeathEventHandler OnDeath;


	private async void WaitForImmunityFrames(DamageInstance a) {
		IsImmune = true;
		GD.Print("Is immune");
		await Task.Delay((int)(ImmunityFrames * 1000));
		IsImmune = false;
		GD.Print("Is no longer immune");
	}

	private Damageable() {
		MaxHealth = Health;
		Ondamaged += WaitForImmunityFrames;
	}
	
	public void Damage(DamageInstance damageInstance) {
		if (IsImmune) return;

		Health -= damageInstance.damage;
		Ondamaged?.Invoke(damageInstance);
		if (Health <= 0) {
			OnDeath?.Invoke(damageInstance);
		}
	}

	public void OnBodyEntered(Node body) {
		GD.Print("wait what");
	}
}


public class DamageInstance : Object{
	public int damage = 0;
	public bool isGrounded = true;
	public Vector2 forceDirection;

	public DamageInstance() {}
	//e.g
}
