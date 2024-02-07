
using System;
using Game.Damage;
using Godot;

namespace Game.Bullets;


public abstract class BaseBullet {
    public event Action OnCollided; 

    Node2D sceneNode;
    DamageInstance damageInstance;

    int speed;
    Vector2 directionFacing;

    public void Create(Area2D area2D, Node2D sceneNode, DamageInstance damageInstance, int speed, float rotation) {
        this.sceneNode = sceneNode;
        this.damageInstance = damageInstance;
        
        area2D.BodyEntered += OnBodyEntered;
        area2D.AreaEntered += OnArea2DEntered;      

        
        this.speed = speed;
        directionFacing = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation));

        // damageInstance = GetBulletDamageInstance(bulletInfo, directionFacing);

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



// public record BulletInstance (BulletFrom from, BulletSpeed speed, DamageInstance damage);

public record BulletTemplate (BaseBullet BaseBullet, BulletFrom From, BulletSpeed Speed, DamageInstance Damage, BulletVisual Visual, Vector2 SpawnPosition, float Rotation);

/// <summary>
/// Defined actors can summon bullets. Therefor, each enum will hold the layers/mask that the 
/// summoner would hit. For example, a player would hit enemies. Enemies would hit players.
/// </summary>
public enum BulletFrom {
    Player,
    Enemy,
}

public enum BulletSpeed : uint {
    VerySlow = 100,
    KindaSlow = 250,
    Slow = 500,
    Fast = 1000,
    VeryFast = 1500,
    Instant = 2000,
}

/*

class factory {

    readonly PackedScene baseBulletStuff = ResourceLoader.Load<PackedScene>("res://source/weapons/bullets/base_bullet.tscn");

    void SpawnBulletPattern(IPattern pattern) {
        pattern.StartPattern(); // might be redundant; a direct call from the pattern might be better (?)
    }

    void SpawnBullet(BulletTemplate bulletTemplate) {  
        Node2D baseNode = baseBulletStuff.Instantiate<Node2D>();
        Area2D area2D = baseNode.GetNode<Area2D>("Area2D");

        bulletTemplate.BaseBullet.Create(area2D, baseNode, bulletTemplate.Damage, (int) bulletTemplate.Speed, bulletTemplate.Rotation);

        DoSafetyChecks(area2D);
        UpdateBulletCollision(bulletTemplate.From, area2D);


        // delte the visual after the bullet is deleted via "OnCollided"
    }

    private static void DoSafetyChecks(Area2D hitbox) {
        if (hitbox.CollisionLayer != 0 || hitbox.CollisionMask != 0) {
            GD.PushError( $"The hitbox {hitbox.Name} for {hitbox.GetParent().Name} has collisions/masks already set. Default them to have nothing.");
            throw new Exception("Amazing");
        }
    }

    private static void UpdateBulletCollision(BulletFrom from, Area2D hitbox) {
        //Force the layers & masks to be overrided by the init
        
        switch (from) {
            case BulletFrom.Player:
                hitbox.CollisionLayer += (int) Layers.PlayerProjectile;
                hitbox.CollisionMask += (int) Layers.Environment + (int) Layers.Enemies;
                break;
            case BulletFrom.Enemy:
                hitbox.CollisionLayer += (int) Layers.EnemyProjectile;
                hitbox.CollisionMask += (int) Layers.Environment + (int) Layers.Player;
                break;
        }
    }

}

*/