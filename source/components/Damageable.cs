using Godot;
using System;
using System.Threading.Tasks;
using KidoUtils;
using Game.Autoload;
using Game.Graphics;
using Game.Actors;


namespace Game.Damage;

public partial class Damageable : Area2D {
	[Export]
	public int Health {get; private set;}
	[Export(PropertyHint.None, "In seconds: how long it takes for this entity to be hit again")]
	private double ImmunityFrames {get; set;}
	
	public ModifiedStat maxHealth = new();
	public int EffectiveHealth => (int) maxHealth.GetEffectiveValue(BaseMaxHealth);
	public int BaseMaxHealth {get; init;}

	public bool BlocksDamage {get; set;} = false;
	public bool IsImmune {get; private set;} = false;

	
	public ModifiedStat damageTaken = new();

    public ModifiedStat regenSpeed = new();

	public bool IsAlive => Health > 0;

	public event Action<DamageInstance> OnDamaged;
	public event Action<int> TotalDamageTaken;
	public event Action<DamageInstance> DamagedBlocked;

	public Action<DamageInstance> OnDeath;

	public override void _Ready() => ErrorUtils.AvoidEmptyCollisionLayers(this);

	private async void WaitForImmunityFrames(DamageInstance a) {
		
		IsImmune = true;
		await Task.Delay((int)(ImmunityFrames * 1000));
		IsImmune = false;
	}

	private Damageable() {
		BaseMaxHealth = Health;

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
	private int GetTotalDamage(DamageInstance damageInstance) {
		float effectiveDamageDealt = damageInstance.damageDealt.GetEffectiveValue(damageInstance.damage);
		float effectiveDamageAbsorbed = damageTaken.GetEffectiveValue(1);

		return (int) MathF.Round(effectiveDamageAbsorbed * effectiveDamageDealt);
	}
	
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

		if (totalDamage < 0) return;

		// This is for shielding. Make sure to deal with damage that goes thru  shields tho
		if (BlocksDamage) {
			DamagedBlocked?.Invoke(damageInstance);
			return;
		}

		Health -= totalDamage;
		OnDamaged?.Invoke(damageInstance);
		TotalDamageTaken?.Invoke(totalDamage);
		
		SpawnDamageText(totalDamage);

		if (Health <= 0) OnDeath?.Invoke(damageInstance);
	}

	public void Kill(DamageInstance damageInstance) {
		OnDamaged?.Invoke(damageInstance);
		OnDeath?.Invoke(damageInstance);
	}
}

