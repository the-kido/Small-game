using Godot;
using Game.Bullets;

namespace Game.Autoload;

public partial class BulletFactory : Node {
    public Bullet SpawnBullet(PackedScene bulletPrototype) {
        
        Bullet newBullet = (Bullet) bulletPrototype.Instantiate();
        
        AddChild(newBullet);
        
        if (newBullet is not Bullet) {
            GD.PushError("The PackedScene", bulletPrototype.ResourcePath, "was not of type Bullet");
            return null;
        }
        return newBullet;
    }
}