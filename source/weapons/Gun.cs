using Godot;
using KidoUtils;
using Game.Damage;
using Game.Autoload;
using System;

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

