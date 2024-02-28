using Godot;
using Game.Bullets;
using KidoUtils;
using System.Collections.Generic;
using Game.Graphics;
using Game.LevelContent;

namespace Game.Autoload;

public partial class BulletFactory : Node {

    static BulletFactory instance;
    public override void _Ready() {
        SceneSwitcher.SceneSwitched += GetRidOfEverything;
        instance = this;
    }
    
    private void GetRidOfEverything() {
        foreach (BaseBullet bullet in bullets) {
            bullet.DestroyBullet();
        }

        bullets.Clear();
    }

    public override void _Process(double delta) {
        foreach (BaseBullet bullet in bullets) {
            bullet.Update(delta);
        }
    } 

    

    static readonly PackedScene baseBulletStuff = ResourceLoader.Load<PackedScene>("res://source/weapons/bullets/base_bullet.tscn");

    public static void SpawnBulletPattern(BulletPattern pattern) {
        pattern.StartPattern(); // might be redundant; a direct call from the pattern might be better (?)
    }

    static readonly List<BaseBullet> bullets = new();
    
    public static void SpawnBullet(BulletTemplate template) {  
        Node2D baseNode = baseBulletStuff.Instantiate<Node2D>();
        BaseBullet baseBullet = BaseBullet.New(template.BaseBullet);
        BulletVisual bulletVisual = BulletVisual.New(template.Visual);

        baseNode.TreeExiting += () => bullets.Remove(baseBullet);
        instance.AddChild(baseNode);

        // Initialize the bullet 
        Area2D area2D = baseNode.GetNode<Area2D>("Area2D");

        baseBullet.Create(area2D, baseNode, template.Damage, (int) template.Speed, template.Rotation);

        

        baseNode.AddChild(bulletVisual);

        // Initialize other values required
        baseNode.Position = template.SpawnPosition;
        baseNode.Rotation = template.Rotation;
        DoSafetyChecks(area2D);
        UpdateBulletCollision(template.From, area2D);

        
        bullets.Add(baseBullet);
        
        // Spawn the persistent particle
        if (bulletVisual.persistentParticle is not Effects.Persistent.None) 
            ParticleFactory.AddFollowingParticle(Effects.GetEffect(bulletVisual.persistentParticle), baseNode);

        baseNode.TreeExiting += () => ParticleFactory.AddInstantParticle(Effects.GetEffect(bulletVisual.deathParticle), baseNode.GlobalPosition, template.Rotation + Mathf.DegToRad(90));
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