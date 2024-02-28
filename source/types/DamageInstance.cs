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
	public ActorStatus statusEffect = null;

	public ModifiedStat damageDealt;
	public DamageInstance(Actor actor, Type type = Type.Normal) {
		damageDealt = actor.DamageDealt;
		this.type = type;
	}
}
