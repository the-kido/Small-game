using Godot;
using Game.Damage;
using Game.Bullets;
using Game.ActorStatuses;

public partial class WeirdGun : Gun {
    public override Type WeaponType {get; protected set;} = Type.InstantShot;
    
    public static readonly PackedScene PackedSceneResource = ResourceLoader.Load<PackedScene>("res://assets/weapons/WeirdGun.tscn");
    public override PackedScene PackedScene => PackedSceneResource;

    protected override DamageInstance Damage => new(Player) {
        damage = 1, 
        statusEffect = new WetStatus()
    };

    public override string Description => 
    $@"I don't even know man
    
Damage: {1}
Reload Speed: {BaseReloadSpeed}
    ";

    public override void Attack() => SpawnBulletInstance();

    public override void _Process(double delta) => reloadTimer += delta;
    
    protected override void OnWeaponUsing(double delta) {
        if (reloadTimer >= EffectiveReloadSpeed) {
			reloadTimer = 0;
            Attack();
        }
    }
}
