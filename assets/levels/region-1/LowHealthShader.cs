using Godot;
using Game.Players;
using Game.Damage;

public partial class LowHealthShader : ColorRect {
	
	// Called when the node enters the scene tree for the first time.
	public void Init(Player player) {
		player.DamageableComponent.OnDamaged += (_) => Damaged();
		player.DamageableComponent.OnHealed += (_) => Damaged();
		shaderMaterial = Material as ShaderMaterial;
	}

	enum Animation {
		Increase,
		Decrease,
		Neither,
	}

	private Animation animation = Animation.Neither;

	private float animationFactor = 0;
	private ShaderMaterial shaderMaterial;
	private void UpdateShaderMaterialValue() => shaderMaterial.SetShaderParameter("starting", animationFactor);

	private void Damaged() {
		Damageable damageable = Player.Players[0].DamageableComponent;
		
		GD.Print(damageable.Health , "Healhte");
		bool lowHealth = 1.0 * damageable.Health / damageable.EffectiveMaxHealth <= 0.9;

		if (lowHealth && animationFactor < 1) {
			animation = Animation.Increase;
			Visible = true;
		}
		else if(!lowHealth && animationFactor > 0) {
			animation = Animation.Decrease;
		}
    }


	private void PlayIncreaseAnimation(double delta) {
		if (animationFactor >= 1) {
			animation = Animation.Neither;
			return;
		}
		animationFactor += (float) delta / 2;
		UpdateShaderMaterialValue();
	}

	private void PlayDecreaseAnimation(double delta) {
		if (animationFactor <= 0) {
			animation = Animation.Neither;
			Visible = false;
			return;
		}
		animationFactor -= (float) delta / 2;
		UpdateShaderMaterialValue();
	}

    public override void _Process(double delta) {
		if (animation is Animation.Increase) PlayIncreaseAnimation(delta);
		if (animation is Animation.Decrease) PlayDecreaseAnimation(delta);
    }
}
