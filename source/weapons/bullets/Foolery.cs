
using System;
using Game.Damage;
using Godot;
using KidoUtils;

namespace Game.Bullets;


public abstract class BaseBullet {
    public event Action OnCollided; 

    readonly Node2D sceneNode;
    readonly DamageInstance damageInstance;

    BaseBullet(Area2D area2D, Node2D sceneNode, DamageInstance damageInstance) {
        this.sceneNode = sceneNode;
        this.damageInstance = damageInstance;
        
        area2D.BodyEntered += OnBodyEntered;
        area2D.AreaEntered += OnArea2DEntered;        
    }

    private void OnArea2DEntered(Area2D area) {
        if (area is Damageable damageable) {
           damageable.Damage(damageInstance);
           DestroyBullet();
        }
    }

    public virtual void DestroyBullet() {
        
        /* Factory will deal with deleting the visuals.
        OnCollided?.Invoke();
        SpawnDestroyedParticle();
        */
        OnCollided?.Invoke();
        sceneNode.QueueFree();
    }
    
    private void OnBodyEntered(Node2D body) {
        if (body is TileMap) {
            DestroyBullet();
        }
    }
}


public interface IPattern {
    // called by bullet factory when bullet is created
    
    BulletTemplate[] BulletTemplates {get;}

    public void StartPattern();
    public void UpdatePattern();
    
}


public partial class BulletVisual : Sprite2D {
    
    [Export]
    GpuParticles2D deathParticle;

    [Export]
    GpuParticles2D persistentParticle;

}

public record BulletInstance (BulletFrom from, BulletSpeed speed, DamageInstance damage);

public record BulletTemplate (BulletFrom From, BulletSpeed Speed, DamageInstance Damage, BulletVisual Visual);


public enum BulletSpeed : uint {
    VerySlow = 100,
    KindaSlow = 250,
    Slow = 500,
    Fast = 1000,
    VeryFast = 1500,
    Instant = 2000,
}



class factory {

    readonly PackedScene baseBulletStuff;

    void SpawnBulletPattern(IPattern pattern) {
        pattern.StartPattern(); // might be redundant; a direct call from the pattern might be better (?)
    }

    void SpawnBullet(BulletTemplate bulletTemplate) {  

        // delte the visual after the bullet is deleted via "OnCollided"
    }

}
