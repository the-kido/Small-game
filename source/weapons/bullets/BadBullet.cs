using Godot;
using System;
using System.Threading.Tasks;

public partial class BadBullet : Bullet {
    
    [Export]
    private float wiggleStrength;
    public double wiggleDistance;
    
    public override void _Ready() {
        Random random = new();
        wiggleDistance = (random.Next( (int) wiggleStrength * 2)) - wiggleStrength;
    }

    public override void _Process(double delta)
    {  
        MoveBulletForward(delta);
        wigglewiggle(delta);
    }

    //this is techically wrong
    private void wigglewiggle(double delta) {

        wiggleDistance += delta * 10;
        float magnitude = wiggleStrength * ((float) Mathf.Sin(wiggleDistance)); 

        Vector2 direction = directionFacing.Rotated(1.5708f).Normalized();
        Vector2 vector = direction * magnitude;

        RotationDegrees += wiggleStrength * ((float) Mathf.Sin(wiggleDistance));

        Position += vector;
    }
    public override void DestroyBullet() {
        QueueFree();
        SpawnDestroyedParticle();
    }   

    public override void OnTilemapEntered(TileMap tileMap)
    {
        DestroyBullet();
    }
    public override void OnDamageableEntered(Damageable damageable, DamageInstance damageInstance)
    {
        damageable.Damage(damageInstance);
        DestroyBullet();
    }
    
}