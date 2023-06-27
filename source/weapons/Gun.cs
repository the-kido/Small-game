using Godot;
using KidoUtils;


public abstract partial class Gun : Weapon {
    [Export]
    private PackedScene bulletAsset;
    [Export]
    protected Node2D nuzzle;

    protected abstract DamageInstance damage {get;}
    protected abstract BulletInstance BulletInstance();
    
    protected void SpawnBulletInstance() => 
            Utils.GetPreloadedScene<BulletFactory>(this, PreloadedScene.BulletFactory) 
            .SpawnBullet(bulletAsset)
            .Init(nuzzle.GlobalPosition, nuzzle.GlobalRotation, BulletInstance());

    public override void UpdateWeapon(Vector2 attackDirection) {
        Hand.LookAt(attackDirection);
    }
}

