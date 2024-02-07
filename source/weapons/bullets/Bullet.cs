using Godot;
using System;
using System.Threading.Tasks;
using KidoUtils;
using Game.Autoload;
using Game.Damage;

namespace Game.Bullets;

public abstract partial class Bullet : Node2D { 

    private DamageInstance damageInstance;

    [Export(PropertyHint.Enum)]
    private BulletSpeed speed;
    [Export]
    private GpuParticles2D particles;
    [Export]
    private int particleDeletionTime;
    [Export]
    private Area2D hitbox;

    protected Vector2 directionFacing;

    protected event Action OnCollided;
    

    #region abstract classes to inherit from
    
    public virtual void OnDamageableEntered(Damageable damageable, DamageInstance damageInstance) {
        damageable.Damage(damageInstance);
        DestroyBullet();
    }
    
    public virtual void OnTilemapEntered(TileMap tileMap) {
        DestroyBullet();
    }
    
    public virtual void DestroyBullet() {
        OnCollided?.Invoke();
        SpawnDestroyedParticle();
        QueueFree();
    }
    #endregion

    private void OnArea2DEntered(Area2D area) {
        if (area is Damageable damageable) {
            OnDamageableEntered(damageable, damageInstance);
        }
    }
    
    private void OnBodyEntered(Node2D body) {
        if (body is TileMap tileMap) {
            OnTilemapEntered(tileMap);
        }
    }
    
    public async void SpawnDestroyedParticle() {
        var newParticle = ParticleFactory.SpawnGlobalParticle(particles, GlobalPosition, GlobalRotation + 90);
        await Task.Delay(particleDeletionTime * 1000);
        newParticle.QueueFree();
    }

    private DamageInstance GetBulletDamageInstance(BulletInstance bulletInfo, Vector2 direction) {
        DamageInstance damageInstance = bulletInfo.damage;
        damageInstance.forceDirection = direction; 
        return damageInstance;
    }

    public void Init(Vector2 spawnPosition, float radians, BulletInstance bulletInfo) {
        //Attach events
        hitbox.AreaEntered += OnArea2DEntered;
        hitbox.BodyEntered += OnBodyEntered;

        //Force the layers & masks to be overrided by the init
        if (hitbox.CollisionLayer != 0 || hitbox.CollisionMask != 0) {
            GD.PushError( $"The hitbox {hitbox.Name} for {hitbox.GetParent().Name} has collisions/masks already set. Default them to have nothing.");
            throw new Exception("Amazing");
        }

        switch (bulletInfo.from) {
            case BulletFrom.Player:
                hitbox.CollisionLayer += (int) Layers.PlayerProjectile;
                hitbox.CollisionMask += (int) Layers.Environment + (int) Layers.Enemies;
                break;
            case BulletFrom.Enemy:
                hitbox.CollisionLayer += (int) Layers.EnemyProjectile;
                hitbox.CollisionMask += (int) Layers.Environment + (int) Layers.Player;
                break;
        }

        Rotation = radians;
        speed = bulletInfo.speed;
        directionFacing = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

        damageInstance = GetBulletDamageInstance(bulletInfo, directionFacing);

        Position = spawnPosition;
    }

    public void MoveBulletForward(double delta) {
        Position += directionFacing * (float) delta * (int) speed;
    }

    public override void _PhysicsProcess(double delta) {
        MoveBulletForward(delta);
    }
}

