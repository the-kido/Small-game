using Godot;
using System;
using System.Threading.Tasks;
using KidoUtils;


public partial class Damageable : Area2D
{
	//Do everything a damageable entity would need
	[Export]
	public int Health {get; private set;}
	[Export(PropertyHint.None, "In seconds: how long it takes for this entity to be hit again")]
	private double ImmunityFrames {get; set;}
	
	private int MaxHealth {get; init;}
	public bool IsImmune {get; private set;} = false;

	public bool IsAlive {
        get {
            return Health <= 0 ? false : true;
        }
    }

	public delegate void HealthDepletedEventHandler(DamageInstance damageInstance);
	public event HealthDepletedEventHandler OnDamaged;
	public delegate void DeathEventHandler(DamageInstance damageInstance);
	public event DeathEventHandler OnDeath;

	public event Action SetToImmune;
	public event Action SetToUnimmune;

	public override void _Ready() {
		ErrorUtils.AvoidEmptyCollisionLayers(this);

		SetToImmune += () => IsImmune = true;
        SetToUnimmune += () => IsImmune = false;
	}
	
	private async void WaitForImmunityFrames(DamageInstance a) {
		SetToImmune?.Invoke();

		await Task.Delay((int)(ImmunityFrames * 1000));
		
		SetToUnimmune?.Invoke();
	}

	private Damageable() {
		MaxHealth = Health;
		OnDamaged += WaitForImmunityFrames;
	}
	
	public void Damage(DamageInstance damageInstance) {
		if (IsImmune) return;

		Health -= damageInstance.damage;
		OnDamaged?.Invoke(damageInstance);
		if (Health <= 0) {
			OnDeath?.Invoke(damageInstance);
		}
	}


	#region disallow bullets to pass actor when going through immunity frames
	


	#endregion

}


public class DamageInstance {
	public int damage = 0;
	public bool isGrounded = true;
	public Vector2 forceDirection;

	public DamageInstance() {}
	//e.g
}
