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
    protected Vector2 directionFacing;

    public delegate void BulletCollisionEventHandler();
    public event BulletCollisionEventHandler OnBulletDestroyed;

    private void OnArea2DEntered(Rid area_rid, Area2D area, int area_shape_index, int local_shape_index) {

        if (area is Damageable enemy) {
            enemy.Damage(new DamageInstance() {damage = damage, forceDirection = directionFacing});
            DestroyBullet();
        }
    }
    private void OnBodyEntered(Node2D body) {
        if (body is TileMap tileMap) {
            OnBulletDestroyed?.Invoke();
            DestroyBullet();
        }
    }
    public virtual async void DestroyBullet() {
        
        var newParticle = ParticleFactory.SpawnGlobalParticle(particles, GlobalPosition, GlobalRotation + 90);
        QueueFree();
        await Task.Delay(particleDeletionTime * 1000);
        newParticle.QueueFree();
    }

    public void init(Vector2 spawnPosition, float nuzzleRotation) {
        Rotation = nuzzleRotation;
        directionFacing = new Vector2(Mathf.Cos(nuzzleRotation), Mathf.Sin(nuzzleRotation));
        Position = spawnPosition;
    }
    
    public override void _Process(double delta) {
        Position += directionFacing * (float) delta * speed;
    }
 }
