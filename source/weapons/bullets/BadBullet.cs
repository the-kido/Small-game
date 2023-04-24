using Godot;
using System;
using System.Threading.Tasks;

public partial class BadBullet : Bullet {
    
    [Export]
    private float wiggleStrength;
    public double test = 1;
    public override void _Process(double delta)
    {  
        wigglewiggle(delta);

        base._Process(delta);
    }

    //this is techically wrong
    private void wigglewiggle(double delta) {

        test += delta * 10;
        float magnitude = wiggleStrength * ((float) Mathf.Sin(test)); 

        Vector2 direction = directionFacing.Rotated(1.5708f).Normalized();
        Vector2 vector = direction * magnitude;

        RotationDegrees += wiggleStrength * ((float) Mathf.Sin(test));

        Position += vector;
    }
}