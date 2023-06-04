using System;
using Godot;
using KidoUtils;


public partial class ChargedGun : Weapon {

    [Export]
    PackedScene bulletAsset;

    public override Type WeaponType {get; protected set;} = Type.HoldToCharge;

    int maxDamage = 10;
    DamageInstance damage = new() {
        statusEffect = new FireEffect(),
    };
    BulletInstance bulletInstance => new(BulletFrom.Player, damage, BulletSpeed.Fast);

    private Node2D nuzzle => (Node2D) GetNode("Nuzzle");
    
    public override void Attack() {
        Bullet bullet = KidoUtils.Utils.GetPreloadedScene<BulletFactory>(this, PreloadedScene.BulletFactory)
            .SpawnBullet(bulletAsset);

        damage.damage = (int) (maxDamage * strength);

        bullet.Init(nuzzle.GlobalPosition, nuzzle.GlobalRotation, bulletInstance);
            
        Camera.currentCamera.StartShake((float) strength * 100, 300, 1);
    }

    float strength;
    public override void OnWeaponLetGo() {

        strength = (float) reloadTimer / ReloadSpeed;
        reloadTimer = 0;
        
        //Dissallow spam
        if (strength < 0.2) return;
        Attack();
    }

    public override void UpdateWeapon(Vector2 attackDirection) {
        hand.LookAt(attackDirection);
    }

    protected override void OnWeaponUsing(double delta) {
		reloadTimer += delta;
        //Make sure the reloadTimer doesn't go past the reload speed for the strenght calculations.
        reloadTimer = MathF.Min( (float) reloadTimer, ReloadSpeed);
        
        DebugHUD.instance.anySlider.Value = reloadTimer / ReloadSpeed * 100;
    }
}
