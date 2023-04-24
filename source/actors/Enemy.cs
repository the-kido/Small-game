using Godot;
using System;

public partial class Enemy : Actor
{
	
	[Export]
	private Sprite2D sprite; 

	public override void _Ready()
	{
	} 
    public override void OnDeath(DamageInstance damageInstance)
    {
		QueueFree();
    }

    public override void OnDamaged(DamageInstance damageInstance)
    {
        throw new NotImplementedException();
    }
}
