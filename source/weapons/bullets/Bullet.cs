using Godot;
using System;
using System.Threading.Tasks;

public partial class Bullet : Node2D { 

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

    private void OnArea2DEntered(Rid area_rid, Area2D area, int area_shape_index, int local_shape_index) {
        if (area is Damageable damageable) {
            damageable.Damage(new DamageInstance() {damage = damage, forceDirection = directionFacing});
            DestroyBullet();
        }
    }

    public delegate void BulletCollisionEventHandler();
    public event BulletCollisionEventHandler OnBulletDestroyed;
    private void OnBodyEntered(Node2D body) {
        if (body is TileMap tileMap) {
            DestroyBullet();
        }
    }
    public virtual async void DestroyBullet() {
        var newParticle = ParticleFactory.SpawnGlobalParticle(particles, GlobalPosition, GlobalRotation + 90);
        QueueFree();
        OnBulletDestroyed?.Invoke();
        await Task.Delay(particleDeletionTime * 1000);
        newParticle.QueueFree();
    }

    public void init(Vector2 spawnPosition, float nuzzleRotation, BulletFrom from) {
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
        
        Rotation = nuzzleRotation;
        directionFacing = new Vector2(Mathf.Cos(nuzzleRotation), Mathf.Sin(nuzzleRotation));
        Position = spawnPosition;

    }
    public void MoveBulletForward(double delta) {
        Position += directionFacing * (float) delta * speed;
    }
   
}
enum Layers: int {
    Enviornment = 1,
    Player = 2,
    Enemies = 4,
    PlayerProjectile = 8,
    EnemyProjectile = 16,
}
/// <summary>
/// Defined actors can summon bullets. Therefor, each enum will hold the layers/mask that the 
/// Summoner would hit. For example, a player would hit enemies. Enemies would hit players.
///  
/// </summary>

public enum BulletFrom {
    Player,
    Enemy,
}
