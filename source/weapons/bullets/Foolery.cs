using System;
using System.Collections.Generic;
using Game.Damage;
using Game.SealedContent;
using Godot;

namespace Game.Bullets;

public abstract class BaseBullet {
    public static BaseBullet New(All type) {
        return (BaseBullet) Activator.CreateInstance(BulletMap[type]);
    }

    private static readonly Dictionary<All, Type> BulletMap = new(){
        {All.Normal, typeof(BadBullet)}
    };

    public enum All {
        Normal,
    }

    protected Node2D sceneNode;
    DamageInstance damageInstance;

    protected int speed;
    protected Vector2 directionFacing;

    public abstract void Update(double delta);

    public void Create(Area2D area2D, Node2D sceneNode, DamageInstance damageInstance, int speed, float rotation) {
        this.sceneNode = sceneNode;
        this.damageInstance = damageInstance;
        
        area2D.BodyEntered += OnBodyEntered;
        area2D.AreaEntered += OnArea2DEntered;      

        this.speed = speed;
        directionFacing = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation));
    }

    private void OnArea2DEntered(Area2D area) {
        if (area is Damageable damageable) {
            damageable.Damage(damageInstance);
            DestroyBullet();
        }
    }

    public virtual void DestroyBullet() {
        sceneNode.QueueFree();
    }
    
    private void OnBodyEntered(Node2D body) {
        if (body is TileMap) {
            DestroyBullet();
        }
    }
}

public abstract class BulletPattern {
    public enum All {
        TrioBulletPattern,
    }

    public static BulletPattern NewUninitialized(All all) {
        return (BulletPattern) Activator.CreateInstance(BulletMap[all]);
    }

    private static readonly Dictionary<All, Type> BulletMap = new(){
        {All.TrioBulletPattern, typeof(TrioBulletPattern)}
    };

    protected BulletTemplate primaryBullet;
    protected BulletTemplate secondaryBullet;
    protected BulletTemplate tertiaryBullet;
    
    public void Init(params BulletTemplate[] templates) {
        primaryBullet = templates[0];
        secondaryBullet = templates[1];
        tertiaryBullet = templates[2];
    }

    public abstract void StartPattern();
    public abstract void UpdatePattern(double delta);
}

public record BulletTemplate (BaseBullet.All BaseBullet, BulletFrom From, BulletSpeed Speed, DamageInstance Damage, BulletVisual.All Visual, Vector2 SpawnPosition, float Rotation);

/// <summary>
/// Defined actors can summon bullets. Therefor, each enum will hold the layers/mask that the 
/// summoner would hit. For example, a player would hit enemies. Enemies would hit players.
/// </summary>
public enum BulletFrom {
    Player,
    Enemy,
}

public enum BulletSpeed : uint {
    VerySlow = 150,
    KindaSlow = 250,
    Slow = 500,
    Fast = 1000,
    VeryFast = 1500,
    Instant = 2000,
}