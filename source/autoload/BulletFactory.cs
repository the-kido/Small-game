using Godot;
using Game.Bullets;

namespace Game.Autoload;

public partial class BulletFactory : Node {

    public override void _Ready() {
        instance = this;
    }

    static BulletFactory instance;

    public static Bullet SpawnBullet(PackedScene bulletPrototype) {
        
        Bullet newBullet = (Bullet) bulletPrototype.Instantiate();
        
        instance.AddChild(newBullet);
        
        if (newBullet is null) {
            GD.PushError("The PackedScene", bulletPrototype.ResourcePath, "was not of type Bullet");
            return null;
        }
        return newBullet;
    }

    public BulletPattern SpawnBulletPattern() {
        return null;
    }
}