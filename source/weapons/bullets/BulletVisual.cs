using System.Collections.Generic;
using Game.Graphics;
using Godot;

public partial class BulletVisual : Node2D {
    public static BulletVisual New(All all) => visuals[all].Instantiate<BulletVisual>();
    
    private static readonly Dictionary<All, PackedScene> visuals = new() {
        {All.EnemySmall, ResourceLoader.Load<PackedScene>("res://assets/bulllets/visuals/small_bullet.tscn")},
        {All.FlameBullet, ResourceLoader.Load<PackedScene>("res://assets/bulllets/visuals/flame_bullet.tscn")},
    };

    public enum All {
        // Enemy bullets
        EnemySmall,
        // Player bullets
        FlameBullet,
    }

    [Export]
    Sprite2D sprite;
    
    [Export]
    public Effects.Instant deathParticle;

    [Export]
    public Effects.Persistent persistentParticle;

}