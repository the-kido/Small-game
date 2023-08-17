using Godot;
using Game.Damage;
using Game.Bullets;

public partial class WeirdGun : Gun {
    public override Type WeaponType {get; protected set;} = Type.InstantShot;
    
    public static PackedScene PackedSceneResource {get; set;} = ResourceLoader.Load<PackedScene>("res://source/weapons/WeirdGun.tscn");
    public override PackedScene PackedScene => PackedSceneResource;

    protected override DamageInstance Damage => new(Player) {damage = 1};

    public override string Description => 
    $@"I don't even know man
    
Damage: {1}
Reload Speed: {ReloadSpeed}
    ";

    public override void Attack() => SpawnBulletInstance();

    public override void _Process(double delta) => reloadTimer += delta;
    
    protected override void OnWeaponUsing(double delta) {
        if (reloadTimer >= ReloadSpeed) {
			reloadTimer = 0;
            Attack();
        }
    }

    protected override BulletInstance BulletInstance() =>
        new(KidoUtils.BulletFrom.Player, Damage, BulletSpeed.Fast);
}
