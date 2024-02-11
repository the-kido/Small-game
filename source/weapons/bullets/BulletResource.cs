using Godot;

namespace Game.Bullets;

[GlobalClass]
public partial class BulletResource : Resource {
	[Export(PropertyHint.Enum)]
    public BaseBullet.All bulletBase;
    [Export(PropertyHint.Enum)]
    public BulletVisual.All visual;
    [Export]
    public BulletSpeed speed;
}
