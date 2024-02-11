using Game.Bullets;

namespace Game.SealedContent;

public sealed partial class BadBullet : BaseBullet {
    public override void Update(double delta) {
        sceneNode.Position += directionFacing * (float) delta * speed;
    }
}