using Godot;
using System;
using System.Threading.Tasks;
using KidoUtils;

public abstract partial class Bullet : Node2D { 

    [Export]
    private int damage;
    [Export(PropertyHint.Range, "-360,360,1,or_greater,or_less")] 
    private int speed;
    [Export]
    private GpuParticles2D particles;
    [Export(PropertyHint.Range, "0,or_greater")]
    private int particleDeletionTime;
    [Export]
    private Area2D hitbox;

    protected Vector2 directionFacing;

    public delegate void BulletCollisionEventHandler();
    public event BulletCollisionEventHandler OnBulletDestroyed;

    #region abstract classes to inherit from
    
    public abstract void OnDamageableEntered(Damageable damageable, DamageInstance damageInstance);
    public abstract void OnTilemapEntered(TileMap tileMap);
    public abstract void DestroyBullet();
    
    #endregion

    
    private DamageInstance BulletDamageInstance() {
        return new DamageInstance(){
            damage = damage,
            forceDirection = directionFacing,
        };
    }

    private void OnArea2DEntered(Rid area_rid, Area2D area, int area_shape_index, int local_shape_index) {
        if (area is Damageable damageable) {
            OnDamageableEntered(damageable, BulletDamageInstance());
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

    public void init(Vector2 spawnPosition, float radians, BulletFrom from) {
        if (hitbox.CollisionLayer != 0 || hitbox.CollisionMask != 0) {
            GD.PushError( $"The hitbox {hitbox.Name} for {hitbox.GetParent().Name} has collisions/masks already set. Default them to have nothing.");
            throw new Exception("Amazing");
        }

        switch (from) {
            case BulletFrom.Player:
                hitbox.CollisionLayer += (int) Layers.PlayerProjectile;
                hitbox.CollisionMask += (int) Layers.Enviornment + (int) Layers.Enemies;
                break;
            case BulletFrom.Enemy:
                hitbox.CollisionLayer += (int) Layers.EnemyProjectile;
                hitbox.CollisionMask += (int) Layers.Enviornment + (int) Layers.Player;
                break;
        }
        
        Rotation = radians;
        GD.Print("r", radians);

        directionFacing = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        GD.Print("a ", directionFacing);
        Position = spawnPosition;

    }
    public void MoveBulletForward(double delta) {
        Position += directionFacing * (float) delta * speed;
    }
   
}
