using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	// [Export]
	// public int maxHealth {get; private set;}
	// public int health {get; private set;}
	[Export]
	private Sprite2D sprite; 
	[Export]
	private Damageable damageableComponent;

	public override void _Ready()
	{
		sprite = (Sprite2D) GetNode("Sprite");
		damageableComponent.OnDeath += Kill;
	}

	public void Kill(DamageInstance damageInstance) {
		QueueFree();
	}
}
