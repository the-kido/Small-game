using Godot;
using System;

public partial class Enemy : Node2D
{
	[Export]
	public int maxHealth {get; private set;}
	public int health {get; private set;}
	private Sprite2D sprite; 


	public override void _Ready()
	{


		sprite = (Sprite2D) GetNode("Sprite");
		GD.Print(sprite, " for ", Name);
		health = maxHealth;
	}
	//Replace w/ decorator in the future
	public void inflictDamage(int damage) {
		health -= damage;
		if (health <= 0) {
			kill(); 
		}
	}
	public void kill() {
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
