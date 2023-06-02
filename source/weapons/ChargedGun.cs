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

    private Node2D nuzzle => (Node2D) GetNode("Nuzzle");
    
    public override void Attack() {
        Bullet bullet = KidoUtils.Utils.GetPreloadedScene<BulletFactory>(this, PreloadedScene.BulletFactory)
            .SpawnBullet(bulletAsset);

        damage.damage = (int) (maxDamage * strength);
        GD.Print($"{damage.damage} DAMAGE WAS DEALT!");


        bullet.Init(nuzzle.GlobalPosition, nuzzle.GlobalRotation, BulletFrom.Player, damage);
            
        Camera.currentCamera.StartShake((float) strength * 100, 300, 1);
    }

    float strength;
    public override void OnWeaponLetGo() {
        GD.Print("weapon let go!");

        strength = (float) reloadTimer / ReloadSpeed;
        Attack();
        reloadTimer = 0;
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
