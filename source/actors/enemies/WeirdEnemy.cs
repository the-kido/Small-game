using System;
using Godot;
using KidoUtils;


public partial class WeirdEnemy : Enemy {
    [Export]
    private PackedScene spamedBullet;

    double bloop = 0;
    public override void _Process(double delta)
    {
        base._Process(delta);
        bloop += delta;
        what();
    }
    private void what() {
        if (bloop >= 1) {
            ShootConstantly();
            bloop = 0;
        }
    }
    private void ShootConstantly() {
        GetNode<BulletFactory>("/root/BulletFactory").SpawnBullet(spamedBullet).init(Position, 180*MathF.PI/180, BulletFrom.Enemy);
    }
}
