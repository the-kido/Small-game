using KidoUtils;
using System;
using Godot;

public partial class ChargedGun : Gun {
    const int MAX_DAMAGE = 10;
    
    // Overriding relavent properties / methods
    public override Type WeaponType {get; protected set;} = Type.HoldToCharge;

	protected override DamageInstance damage => new(player) {
        statusEffect = new FireEffect(), 
        damage = (int) (MAX_DAMAGE * strength)
    };
	protected override BulletInstance BulletInstance() => new(BulletFrom.Player, damage, BulletSpeed.VeryFast);


    
	public override void Attack() {

        SpawnBulletInstance();
        
        Camera.currentCamera.StartShake(strength * 100, 300, 1);
    }

    float strength = 0;
    public override void OnWeaponLetGo() {
        strength = (float) reloadTimer / ReloadSpeed;
        reloadTimer = 0;
        
        //Dissallow spam
        if (strength < 0.2) return;
        Attack();
    }

    protected override void OnWeaponUsing(double delta) {
		reloadTimer += delta;

        //Make sure the reloadTimer doesn't go past the reload speed for the strenght calculations.
        reloadTimer = MathF.Min( (float) reloadTimer, ReloadSpeed);

        DebugHUD.instance.anySlider.Value = reloadTimer / ReloadSpeed * 100;
    }
} 