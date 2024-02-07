using Godot;
using Game.Bullets;
using KidoUtils;
using System.Collections.Generic;

namespace Game.Autoload;

public partial class BulletFactory : Node {

    static BulletFactory instance;
    public override void _Ready() => instance = this;
    public override void _Process(double delta) => bullets.ForEach(bullet => bullet.Update(delta));


    static readonly PackedScene baseBulletStuff = ResourceLoader.Load<PackedScene>("res://source/weapons/bullets/base_bullet.tscn");

    void SpawnBulletPattern(IPattern pattern) {
        pattern.StartPattern(); // might be redundant; a direct call from the pattern might be better (?)
    }

    static readonly List<BaseBullet> bullets = new();
    
    public static void SpawnBullet(BulletTemplate bulletTemplate) {  
        Node2D baseNode = baseBulletStuff.Instantiate<Node2D>();
        instance.AddChild(baseNode);
        baseNode.AddChild(bulletTemplate.Visual);

        Area2D area2D = baseNode.GetNode<Area2D>("Area2D");

        bulletTemplate.BaseBullet.Create(area2D, baseNode, bulletTemplate.Damage, (int) bulletTemplate.Speed, bulletTemplate.Rotation);

        DoSafetyChecks(area2D);
        UpdateBulletCollision(bulletTemplate.From, area2D);

        baseNode.Position = bulletTemplate.SpawnPosition;
        baseNode.Rotation = bulletTemplate.Rotation;

        bullets.Add(bulletTemplate.BaseBullet);
        baseNode.TreeExited += () => bullets.Remove(bulletTemplate.BaseBullet);


        // delte the visual after the bullet is deleted via "OnCollided"
    }

    private static void DoSafetyChecks(Area2D hitbox) {
        if (hitbox.CollisionLayer != 0 || hitbox.CollisionMask != 0) {
            GD.PushError( $"The hitbox {hitbox.Name} for {hitbox.GetParent().Name} has collisions/masks already set. Default them to have nothing.");
            throw new System.Exception("Amazing");
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