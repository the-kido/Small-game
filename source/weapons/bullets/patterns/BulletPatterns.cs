using Game.Actors;
using Game.Autoload;
using Game.Bullets;
using Game.SealedContent;

public sealed class TrioBulletPattern : BulletPattern {
    

    public override void StartPattern(BulletFrom bulletFrom, Actor actor) {
        BulletFactory.SpawnBullet(BulletTemplates(bulletFrom, actor)[0]);
    }

    protected override void UpdatePattern(double delta) {
        throw new System.NotImplementedException();
    }

    protected override BulletTemplate[] BulletTemplates(BulletFrom from, Actor actor) {
        return new BulletTemplate[] {
            new(
                new BadBullet(),
                from,
                BulletSpeed.Fast,
                new(actor) {damage = 10},
                BulletVisual.New(BulletVisual.All.FlameBullet),
                actor.Position,
                actor.Rotation
            )
        };
    }
}