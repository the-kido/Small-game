using Godot;
using System;
using System.Threading.Tasks;

public partial class BadBullet : Bullet {
    
    [Export]
    private float wiggleStrength;
    public double wiggleDistance;
    
    private BadBullet() {
        wiggleDistance = wiggleStrength;
    }

    public override void _Process(double delta)
    {  
        base._Process(delta);
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
}