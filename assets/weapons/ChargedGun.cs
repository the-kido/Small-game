using KidoUtils;
using System;
using Godot;
using Game.ActorStatuses;
using Game.Bullets;
using Game.Damage;
using Game.UI;

namespace Game.SealedContent;

public sealed partial class ChargedGun : Gun {
    [Export]
    int maxDamage = 3;
    
    // Overriding relavent properties / methods
    public override Type WeaponType {get; protected set;} = Type.HoldToCharge;
    
	public static readonly PackedScene PackedSceneResource = ResourceLoader.Load<PackedScene>("res://assets/weapons/ChargedGun.tscn");
    public override PackedScene PackedScene => PackedSceneResource;

	protected override DamageInstance Damage => new(Player) {
        statusEffect = new FireEffect(), 
        damage = (int) (maxDamage * strength)
    };

    public override string Description => 
    $@"{"Charged Gun".Colored(KidoUtils.Colors.LEGENDARY_RARITY)}
As you hold, this weapon charges. Releasing the weapon early deals less damage, but at full charge it is painful.

Damage: {maxDamage}
Reload Speed: {BaseReloadSpeed}
    ";

	public override void Attack() {
        SpawnBulletInstance();
        Camera.CurrentCamera.StartShake(strength * 100, 300, 1);
    }

    float strength = 0;
    public override void OnWeaponLetGo() {
        strength = (float) reloadTimer / EffectiveReloadSpeed;
        reloadTimer = 0;
        
        //Dissallow spam
        if (strength < 0.2) return;
        Attack();
    }

    protected override void OnWeaponUsing(double delta) {
		reloadTimer += delta;

        //Make sure the reloadTimer doesn't go past the reload speed for the strenght calculations.
        reloadTimer = MathF.Min( (float) reloadTimer, EffectiveReloadSpeed);

        DebugHUD.instance.anySlider.Value = reloadTimer / EffectiveReloadSpeed * 100;
    }
} 