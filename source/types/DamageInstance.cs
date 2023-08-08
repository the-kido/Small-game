using Godot;

public record DamageInstance {
	public int damage = 0;
	public bool isGrounded = true;
	public Vector2 forceDirection = Vector2.Zero;
	public bool overridesImmunityFrames = false;
	public bool suppressImpactFrames = false;
	public IActorStatus statusEffect = null;

	public float damageDealtMultiplier = 1;
	public DamageInstance(Actor actor) {
		damageDealtMultiplier = actor.DamageDealingMultplier;
	}
}
