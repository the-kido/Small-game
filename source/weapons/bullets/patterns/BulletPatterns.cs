using Game.Autoload;
using Game.Bullets;
using Godot;

public sealed class TrioBulletPattern : BulletPattern {
    public override void StartPattern() {
        for (int i = -1; i <= 1; i++) {
            float newRotation = primaryBullet.Rotation + Mathf.Pi/6 * i;

            var temp = primaryBullet with {
                Rotation = newRotation
            };

            BulletFactory.SpawnBullet(temp);
        }
    }

    // Called by bullet factory
    public override void UpdatePattern(double delta) {
        throw new System.NotImplementedException();
    }
}