using Godot;
using Game.Damage;
using Game.Autoload;

namespace Game.Bullets;

public abstract partial class Gun : Weapon {
    [Export]
    protected Node2D nuzzle;
    [Export]
    BulletResource bulletResource;

    protected abstract DamageInstance Damage {get;}
    
    protected void SpawnBulletInstance() =>
        BulletFactory.SpawnBullet(new (
            BaseBullet.New(bulletResource.bulletBase),
            BulletFrom.Player,
            bulletResource.speed, 
            Damage, 
            BulletVisual.New(bulletResource.visual), 
            nuzzle.GlobalPosition, 
            nuzzle.GlobalRotation
        ));
 
    protected void SpawnBulletPattern() =>
        BulletFactory.SpawnBullet(new (
            BaseBullet.New(bulletResource.bulletBase),
            BulletFrom.Player,
            bulletResource.speed, 
            Damage, 
            BulletVisual.New(bulletResource.visual), 
            nuzzle.GlobalPosition, 
            nuzzle.GlobalRotation
        ));
 
    public override sealed void UpdateWeapon(Vector2 positionToAttack) {
        // Vector2 direction = positionToAttack - Hand.GlobalPosition;
        
        // double radians = Math.Atan2(-direction.Y, direction.X);

        // double degrees = radians * (180 / Math.PI) + 45;
        // if (degrees < 0) degrees += 360;

        // double remainder =  (degrees % 90);
        // float a = (float) (remainder - degrees);
        
        // Hand.RotationDegrees = a;

        Hand.LookAt(positionToAttack);
    }
}

