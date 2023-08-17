using Godot;
using Game.ActorStatuses;
using Game.Actors;

namespace Game.Damage;

public record DamageInstance {
	public enum Type {
		Normal,
		ShieldPassable,
		Void
	}

	public Type type;

	public int damage = 0;
	public bool isGrounded = true;
	public Vector2 forceDirection = Vector2.Zero;
	public bool overridesImmunityFrames = false;
	public bool suppressImpactFrames = false;
	public IActorStatus statusEffect = null;

	public float damageDealtMultiplier = 1;
	public DamageInstance(Actor actor, Type type = Type.Normal) {
		damageDealtMultiplier = actor.DamageDealingMultplier;
		this.type = type;
	}
}
