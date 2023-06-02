using Godot;
using System;
using System.Threading.Tasks;

public partial class BadBullet : Bullet {
    
    [Export]
    private float wiggleStrength;
    private double wiggleDistance;
    
    public override void _Ready() {
        Random random = new();
        wiggleDistance = (random.Next( (int) wiggleStrength * 2)) - wiggleStrength;
    }

    public override void _Process(double delta) {  
        base._Process(delta);
        wigglewiggle(delta);
    }

    private void wigglewiggle(double delta) {
        wiggleDistance += delta * 10;
        float magnitude = wiggleStrength * ((float) Mathf.Sin(wiggleDistance)); 

        Vector2 direction = directionFacing.Rotated(1.5708f).Normalized();
        Vector2 vector = direction * magnitude;

        RotationDegrees += wiggleStrength * ((float) Mathf.Sin(wiggleDistance));

        Position += vector;
    }
}