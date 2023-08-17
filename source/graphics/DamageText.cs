using Godot;
using System;

namespace Game.Graphics;

public partial class DamageText : Node2D {
    [Export]
    Label label;

    float xSpeed;

    // Required to find the change in distance
    Vector2 newShift;
    Vector2 previousShift;

    float sinX = 0;
    private void PlayAnimation(double delta) {
        newShift.X += (float) (xSpeed * delta);

        newShift.Y = -50 * MathF.Sin((float) timeAlive * 4f);

        Vector2 shift = new(
            newShift.X - previousShift.X,
            newShift.Y - previousShift.Y
        );

        previousShift = newShift;

        Position += shift;
    }

    double timeAlive;

    public override void _Process(double delta) {
        timeAlive += delta;
        PlayAnimation(delta);
    }

    public void Init(int totalDamage, Vector2 globalPosition) {

        Color color = totalDamage switch {
            < 0 => new(0.7f, 0.95f, 0.61f),     // Heal
            < 3 => new(0.81f, 0.1f, 0.1f),      // Bad
            < 6 => new(0.9f, 0.5f, 0.1f),       // Decent
            < 9 => new(0.95f, 0.75f, 0.3f),     // Tres bien
            < 12 => new(0.87f, 0.61f, 0.97f),   // Tres beaucoup bien ?
            _ => new()
        };
        Modulate = color;

        label.Text = totalDamage.ToString();

        GlobalPosition = globalPosition;

        xSpeed = (new Random().NextSingle() - 0.5f) * 4; 
        xSpeed *= 80;
    }
}
