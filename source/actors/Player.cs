using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Players.Mechanics;
using Game.Actors;
using Game.Players.Inputs;
using Game.UI;
using Game.Damage;
using System;
using System.Diagnostics;
using Game.LevelContent;

namespace Game.Players;

public partial class Player : Actor {
    [ExportCategory("Global properties")]
    [Export]
    public GUI GUI {get; private set;}

    [ExportCategory("Required")]
    [Export]
    public PlayerInteractableRadar InteractableRadar {get; private set;}
    [Export]
    public InputController InputController {get; private set;}
    [Export]
    public WeaponManager WeaponManager;
    [Export]
    public ShieldManager ShieldManager;

    [ExportCategory("Optional")]
    [Export]
    private AudioStreamPlayer2D epicSoundEffectPlayer;

    public static List<Player> Players {get; private set;}

    protected override void UpdateStats(ActorStats newStats) {
        // WeaponManager.reloadSpeed = newStats.reloadSpeed;
        MoveSpeed = newStats.speed;
        DamageableComponent.damageTaken = newStats.damageTaken;
        damageDealt = newStats.damageDealt;
        DamageableComponent.maxHealth = newStats.maxHealth;
        DamageableComponent.regenSpeed = newStats.regenSpeed;
        
        WeaponManager.reloadSpeed = newStats.reloadSpeed;
    }

    public override void _Ready() {
        base._Ready();
        Players = new() { this };
        Camera.currentCamera.Init(this);
        
        InputController.Init(this);
        
        // Init required components
        WeaponManager.Init(this);
        ShieldManager.Init(this);

        GUI.Init(this);

        DamageableComponent.OnDamaged += DamageFramePause;
        DamageableComponent.OnDeath += OnDeath;
    }   
    public void OnDeath(DamageInstance damageInstance) {
        
        GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Playing = true;
        
        PlayImpactFrames(1000);
        Camera.currentCamera.StartShake(300, 300, 2);
        
        //Make player immune to everything and anything all of the time
        CollisionLayer = 0;
        CollisionMask = 0;

        CallDeferred("SetProcessMode", false);
    }
    
    #region death stuff
    private void SetProcessMode(bool enable) =>
        ProcessMode = enable ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
    
    public void DamageFramePause(DamageInstance damageInstance) {
        if (!damageInstance.suppressImpactFrames) PlayImpactFrames(300);
    }

    private async void PlayImpactFrames(int milliseconds) {
        GetTree().Paused = true;
        await Task.Delay(milliseconds);
        GetTree().Paused = false;
    }
    #endregion
}
