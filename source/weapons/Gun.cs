using Godot;
using KidoUtils;
using Game.Damage;
using Game.Autoload;

namespace Game.Bullets;

public abstract partial class Gun : Weapon {
    [Export]
    private PackedScene bulletAsset;
    [Export]
    protected Node2D nuzzle;


    protected abstract DamageInstance Damage {get;}
    protected abstract BulletInstance BulletInstance();
    
    protected void SpawnBulletInstance() => 
            Utils.GetPreloadedScene<BulletFactory>(this, PreloadedScene.BulletFactory) 
            .SpawnBullet(bulletAsset)
            .Init(nuzzle.GlobalPosition, nuzzle.GlobalRotation, BulletInstance());

    public override void UpdateWeapon(Vector2 attackDirection) {
        Hand.LookAt(attackDirection);
    }
}

