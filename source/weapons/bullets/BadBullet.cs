using Godot;
using System;
using Game.Bullets;
using Game.Autoload;
using Game.Graphics;
using Game.UI;

namespace Game.SealedContent;

public sealed partial class BadBullet : BaseBullet {
    /*
    
    [Export]
    private float wiggleStrength;
    private double wiggleDistance;
    
    public override void _Ready() {
        Node2D particle = ParticleFactory.SpawnGlobalFolliwngParticle(Effects.Fire, this);

        OnCollided += () => {
            ParticleFactory.RemoveParticle(particle);
            
            // Turns off smoke (is deleted in partical factory)
            particle.GetChild<GpuParticles2D>(0).Emitting = false;
            Camera.currentCamera.StartShake(50, 300, 1);

        };

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
    */
}