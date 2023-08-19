using Godot;
using System;
using System.Threading.Tasks;
using KidoUtils;
using Game.Autoload;
using Game.Graphics;

namespace Game.Damage;

public partial class Damageable : Area2D {
	[Export]
	public int Health {get; private set;}
	[Export(PropertyHint.None, "In seconds: how long it takes for this entity to be hit again")]
	private double ImmunityFrames {get; set;}
	
	public int MaxHealth {get; init;}
	
	public bool BlocksDamage {get; set;} = false;
	public bool IsImmune {get; private set;} = false;

	private float _damageTakenMultiplier = 1f;
	public float DamageTakenMulitplier {
		get => _damageTakenMultiplier;
		set {
			if (value < 0)
				throw new ArgumentOutOfRangeException(nameof(value), "Range must be above 0");
		
			_damageTakenMultiplier = value;
		}
	}

	public bool IsAlive => Health > 0;

	public event Action<DamageInstance> OnDamaged;
	public event Action<DamageInstance> DamagedBlocked;

	public Action<DamageInstance> OnDeath;

	public override void _Ready() => ErrorUtils.AvoidEmptyCollisionLayers(this);

	private async void WaitForImmunityFrames(DamageInstance a) {
		
		IsImmune = true;
		await Task.Delay((int)(ImmunityFrames * 1000));
		IsImmune = false;
	}

	private Damageable() {
		MaxHealth = Health;

		OnDamaged += WaitForImmunityFrames;

		// QueueFree in order to not take more damage
		OnDeath += (_) => QueueFree();
	}
	
	private static readonly PackedScene damageText = ResourceLoader.Load<PackedScene>("res://source/autoload/damage_text.tscn");

	private DamageText SpawnDamageText(int damage) {
		
		DamageText instance = damageText.Instantiate<DamageText>();
		
		instance.Init(damage, GlobalPosition);
		
		DamageTextManager.instance.AddDamageText(instance);

		return instance;
	}
	private int GetTotalDamage(DamageInstance damageInstance) =>
		(int) MathF.Round(damageInstance.damage * _damageTakenMultiplier * damageInstance.damageDealtMultiplier);
	
	private bool CanTakeDamage(DamageInstance damageInstance) {
		// Or not, maybe just allow a dead entity to take damage. Who am I to judge.
		if (!IsAlive) return false;

		//If the actor is immune and the damage instance cannot override immunity frames, continue.
		if (IsImmune && !damageInstance.overridesImmunityFrames) return false;

		// it passes the test. pog champ
		return true;
	}

	public void Damage(DamageInstance damageInstance) {
		if (!CanTakeDamage(damageInstance)) return;

		int totalDamage = GetTotalDamage(damageInstance);

		// This is for shielding. Make sure to deal with damage that goes thru  shields tho
		if (BlocksDamage) {
			DamagedBlocked?.Invoke(damageInstance);
			return;
		}

		Health -= totalDamage;
		OnDamaged?.Invoke(damageInstance);
		
		SpawnDamageText(totalDamage);

		if (Health <= 0) OnDeath?.Invoke(damageInstance);
	}

	public void Kill(DamageInstance damageInstance) {
		OnDamaged?.Invoke(damageInstance);
		OnDeath?.Invoke(damageInstance);
	}
}

