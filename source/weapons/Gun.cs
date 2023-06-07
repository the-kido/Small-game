using Godot;
using KidoUtils;


public abstract partial class  Gun : Weapon {
    [Export]
    private PackedScene bulletAsset;

    protected abstract DamageInstance damage {get; init;}
    protected abstract BulletInstance BulletInstance();
    
    protected Node2D nuzzle => (Node2D) GetNode("Nuzzle");
    protected void SpawnBulletInstance() => 
            KidoUtils.Utils.GetPreloadedScene<BulletFactory>(this, PreloadedScene.BulletFactory) 
            .SpawnBullet(bulletAsset)
            .Init(nuzzle.GlobalPosition, nuzzle.GlobalRotation, BulletInstance());

    public override void UpdateWeapon(Vector2 attackDirection) {
        hand.LookAt(attackDirection);
    }
}

