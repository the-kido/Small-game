using Godot;
using Game.Damage;
using Game.Autoload;

namespace Game.Bullets;

public abstract partial class Gun : Weapon {
    [Export]
    protected Node2D nuzzle;
    [Export]
    BulletResource bulletResource;
    [Export]
    BulletPatternResource bulletPatternResource;
    [Export]
    Godot.Collections.Array<BulletResource> bulletPatternResources;

    protected abstract DamageInstance Damage { get; }

    protected void SpawnBulletInstance() =>
        BulletFactory.SpawnBullet(new(
            bulletResource.bulletBase,
            BulletFrom.Player,
            bulletResource.speed,
            Damage,
            bulletResource.visual,
            nuzzle.GlobalPosition,
            nuzzle.GlobalRotation
        )
    );

    private BulletTemplate GetBulletTemplate(int index) => bulletPatternResource.bulletResources.Count > index 
    ? new(
        bulletPatternResource.bulletResources[index].bulletBase,
        BulletFrom.Player,
        bulletPatternResource.bulletResources[index].speed,
        Damage,
        bulletPatternResource.bulletResources[index].visual,
        nuzzle.GlobalPosition,
        nuzzle.GlobalRotation
    ) : null;
    
    protected void SpawnBulletPattern() => BulletFactory.SpawnBulletPattern(GetBulletPattern());
    BulletPattern GetBulletPattern() {
        var pattern = BulletPattern.NewUninitialized(BulletPattern.All.TrioBulletPattern);
        pattern.Init(GetBulletTemplate(0), GetBulletTemplate(1), GetBulletTemplate(2));
        return pattern;
    }

    public override sealed void UpdateWeapon(Vector2 positionToAttack) {
        Hand.LookAt(positionToAttack);
    }
}
