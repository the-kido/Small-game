using Godot;
using System;

public partial class WeirdGun : Gun
{
    public override Type WeaponType {get; protected set;} = Type.InstantShot;

    
    public static PackedScene Temp {get; set;} = ResourceLoader.Load<PackedScene>("res://source/weapons/WeirdGun.tscn");

    protected override DamageInstance damage => new(Player) {damage = 1};

    public override void Attack() {
        SpawnBulletInstance();
    }

    protected override BulletInstance BulletInstance() {
        return new(KidoUtils.BulletFrom.Player, damage, BulletSpeed.Fast);
    }
}
