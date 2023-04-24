using Godot;
using System;

public partial class Enemy : Node2D
{
	// [Export]
	// public int maxHealth {get; private set;}
	// public int health {get; private set;}
	private Sprite2D sprite; 
	[Export]
	private Damageable damageableComponent;
	[Export]
	public Area2D debug;

	public override void _Ready()
	{
		sprite = (Sprite2D) GetNode("Sprite");
		GD.Print(debug.CollisionLayer);
		damageableComponent.OnDeath += Kill;

	}
	//Replace w/ decorator in the future
	public void Kill(DamageInstance damageInstance) {
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void _on_area_2d_body_entered(Node body) {
		GD.Print("what is ", body.Name);
	}
}
