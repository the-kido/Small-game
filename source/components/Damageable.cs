using Godot;
using System;
using System.Threading.Tasks;
using KidoUtils;


public partial class Damageable : Area2D {
	[Export]
	public int Health {get; private set;}
	[Export(PropertyHint.None, "In seconds: how long it takes for this entity to be hit again")]
	private double ImmunityFrames {get; set;}
	
	private int MaxHealth {get; init;}
	public bool IsImmune {get; private set;} = false;

	private float _damageMultiplier = 1f;
	public float DamageMulitplier {
		get => _damageMultiplier;
		set {
			if (value < 0) {
				throw new ArgumentOutOfRangeException(nameof(value), "Range must be above 0");
			}
			_damageMultiplier = value;
		}
	}


	public bool IsAlive {
        get {
            return Health <= 0 ? false : true;
        }
    }

	public event Action<DamageInstance> OnDamaged;
	public Action<DamageInstance> OnDeath;

	public event Action SetToImmune;
	public event Action SetToUnimmune;

	public override void _Ready() => ErrorUtils.AvoidEmptyCollisionLayers(this);

	private async void WaitForImmunityFrames(DamageInstance a) {
		SetToImmune?.Invoke();
		await Task.Delay((int)(ImmunityFrames * 1000));
		SetToUnimmune?.Invoke();
	}

	private Damageable() {
		MaxHealth = Health;
		
		SetToImmune += () => IsImmune = true;
        SetToUnimmune += () => IsImmune = false;

		OnDamaged += WaitForImmunityFrames;
		OnDeath += (_) => QueueFree();
	}
	
	public void Damage(DamageInstance damageInstance) {
		if (!IsAlive) return;

		//If the actor is immune and the damage instance cannot override immunity frames, continue.
		if (IsImmune && damageInstance.overridesImmunityFrames == false) 
			return;

		Health -= (int)(damageInstance.damage * _damageMultiplier);
		OnDamaged?.Invoke(damageInstance);

		if (Health <= 0) 
			OnDeath?.Invoke(damageInstance);
	}

}

