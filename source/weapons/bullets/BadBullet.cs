using Godot;
using System;
using Game.Bullets;
using Game.Autoload;
using Game.Graphics;

namespace Game.SealedContent;

public sealed partial class BadBullet : Bullet {
    
    [Export]
    private float wiggleStrength;
    private double wiggleDistance;
    
    Node2D particle;
    public override void _Ready() {
        particle = ParticleFactory.SpawnGlobalFolliwngParticle(Effects.Fire, this);
        OnCollided += () => ParticleFactory.RemoveParticle(particle);
        OnCollided += () => particle.GetChild<GpuParticles2D>(0).Emitting = false;

        Random random = new();
        wiggleDistance = random.Next((int) wiggleStrength * 2) - wiggleStrength;
    }

    public override void _Process(double delta) {  
        base._Process(delta);
        Wigglewiggle(delta);
    }

    private void Wigglewiggle(double delta) {
        wiggleDistance += delta * 10;
        float magnitude = wiggleStrength * ((float) Mathf.Sin(wiggleDistance)); 

        Vector2 direction = directionFacing.Rotated(1.5708f).Normalized();
        Vector2 vector = direction * magnitude;

        RotationDegrees += wiggleStrength * ((float) Mathf.Sin(wiggleDistance));

        Position += vector;
    }
}