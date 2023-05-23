using Godot;

public record DamageInstance {
	public int damage = 0;
	public bool isGrounded = true;
	public Vector2 forceDirection = Vector2.Zero;
	public bool overridesImmunityFrames = false;

	public IStatusEffect statusEffect = new NoEffect();

}
