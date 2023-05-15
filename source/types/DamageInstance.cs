using Godot;

public class DamageInstance {
	public int damage = 0;
	public bool isGrounded = true;
	public Vector2 forceDirection;
	public bool overridesImmunityFrames = false;

	public DamageInstance() {}
}