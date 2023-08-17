using KidoUtils;
using System;
using Godot;
using Game.ActorStatuses;
using Game.Bullets;
using Game.Damage;
using Game.UI;

namespace Game.SealedContent;

public sealed partial class ChargedGun : Gun {
    const int MAX_DAMAGE = 10;
    
    // Overriding relavent properties / methods
    public override Type WeaponType {get; protected set;} = Type.HoldToCharge;
    
	public static readonly PackedScene PackedSceneResource = ResourceLoader.Load<PackedScene>("res://source/weapons/BaseGun.tscn");
    public override PackedScene PackedScene {get => PackedSceneResource;}

	protected override DamageInstance Damage => new(Player) {
        statusEffect = new FireEffect(), 
        damage = (int) (MAX_DAMAGE * strength)
    };

    public override string Description => 
    $@"{"Charged Gun".Colored(Colors.LEGENDARY_RARITY)}
As you hold, this weapon charges. Releasing the weapon early deals less damage, but at full charge it is painful.

Damage: {MAX_DAMAGE}
Reload Speed: {ReloadSpeed}
    ";

    protected override BulletInstance BulletInstance() => new(BulletFrom.Player, Damage, BulletSpeed.VeryFast);

	public override void Attack() {
        SpawnBulletInstance();
        Camera.currentCamera.StartShake(strength * 100, 300, 1);
    }

    float strength = 0;
    public override void OnWeaponLetGo() {
        strength = (float) reloadTimer / ReloadSpeed;
        reloadTimer = 0;
        
        //Dissallow spam
        if (strength < 0.2) return;
        Attack();
    }

    protected override void OnWeaponUsing(double delta) {
		reloadTimer += delta;

        //Make sure the reloadTimer doesn't go past the reload speed for the strenght calculations.
        reloadTimer = MathF.Min( (float) reloadTimer, ReloadSpeed);

        DebugHUD.instance.anySlider.Value = reloadTimer / ReloadSpeed * 100;
    }
} 