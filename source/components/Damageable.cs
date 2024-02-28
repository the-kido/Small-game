using Godot;
using System;
using System.Threading.Tasks;
using Game.Autoload;
using Game.Graphics;
using Game.Actors;
using Game.ActorStatuses;
using KidoUtils;

namespace Game.Damage;

public sealed partial class Damageable : Area2D {
	[Export]
	public int Health {get; private set;}
	[Export(PropertyHint.None, "In seconds: how long it takes for this entity to be hit again")]
	private double ImmunityFrames {get; set;}

	[Export(PropertyHint.None, "Actor Statuses that are ignored if they deal damage")]
    public Godot.Collections.Array<AllStatuses> ImmuneToDamageFrom {get; private set;}
	
	public ModifiedStat maxHealth = new();
	public int EffectiveMaxHealth => (int) maxHealth.GetEffectiveValue(BaseMaxHealth);
	public int BaseMaxHealth {get; private set;}

	public bool BlocksDamage {get; set;} = false;
	public bool IsImmune {get; private set;} = false;

	public ModifiedStat damageTaken = new();

    public ModifiedStat regenSpeed = new();

	public bool IsAlive => Health > 0;

	public event Action<DamageInstance> OnDamaged;
	public event Action<int> TotalDamageTaken;
	public event Action<DamageInstance> DamagedBlocked;

	public Action<DamageInstance> OnDeath;

	public override void _Ready() {
		BaseMaxHealth = Health;
		
		ErrorUtils.AvoidEmptyCollisionLayers(this);
	}

	private async void WaitForImmunityFrames(DamageInstance _) {
		
		IsImmune = true;
		await Task.Delay((int)(ImmunityFrames * 1000));
		IsImmune = false;
	}

	private Damageable() {
		OnDamaged += WaitForImmunityFrames;

		OnDeath += (_) => QueueFree(); // QueueFree in order to not take more damage
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

		int effectSynergyBonus = damageInstance.statusEffect?.GetEffectSynergyDamageBonus() ?? 0;

		return (int) MathF.Round(effectiveDamageAbsorbed * effectiveDamageDealt) + effectSynergyBonus;
	}
	
	private bool CanTakeDamage(DamageInstance damageInstance) {
		
		if (!IsAlive) return false; // Or not, maybe just allow a dead entity to take damage. Who am I to judge.

		if (IsImmune && !damageInstance.overridesImmunityFrames) return false; //If the actor is immune and the damage instance cannot override immunity frames, continue.

		return true; // it passes the test. pog champ
	}

	public void ChangeImmunity(bool immune) {
		IsImmune = immune;
	}

	public void Heal(int healthAdded) {
		SpawnDamageText(-healthAdded);
		Health = Mathf.Min(Health + healthAdded, EffectiveMaxHealth);

	}

	public void Damage(DamageInstance damageInstance) {
		
		if (!CanTakeDamage(damageInstance)) 
			return;

		// This is for shielding. Make sure to deal with damage that goes thru shields tho
		if (BlocksDamage) {
			DamagedBlocked?.Invoke(damageInstance);
			return;
		}

		int totalDamage = GetTotalDamage(damageInstance);
		
		if (totalDamage < 0) 
			return;

		Health -= totalDamage;
		
		OnDamaged?.Invoke(damageInstance);
		TotalDamageTaken?.Invoke(totalDamage);
		
		SpawnDamageText(totalDamage);

		if (Health <= 0) 
			OnDeath?.Invoke(damageInstance);
	}

	public void Kill(DamageInstance damageInstance) {
		OnDamaged?.Invoke(damageInstance);
		OnDeath?.Invoke(damageInstance);
	}
}