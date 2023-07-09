using Godot;
using System;

public partial class WeirdGun : Gun {
    public override Type WeaponType {get; protected set;} = Type.InstantShot;
    
    public static PackedScene PackedSceneResource {get; set;} = ResourceLoader.Load<PackedScene>("res://source/weapons/WeirdGun.tscn");
    public override PackedScene PackedScene => PackedSceneResource;

    protected override DamageInstance damage => new(Player) {damage = 1};
 
    public override void Attack() => SpawnBulletInstance();

    public override void _Process(double delta) => reloadTimer += delta;
    
    protected override void OnWeaponUsing(double delta) {
        if (reloadTimer >= ReloadSpeed) {
			reloadTimer = 0;
            Attack();
        }
    }

    protected override BulletInstance BulletInstance() {
        return new(KidoUtils.BulletFrom.Player, damage, BulletSpeed.Fast);
    }
}
