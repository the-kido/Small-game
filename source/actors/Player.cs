using Godot;
using System.Collections.Generic;
using Game.Players.Mechanics;
using Game.Actors;
using Game.Players.Inputs;
using Game.UI;
using Game.Data;

namespace Game.Players;

public sealed partial class Player : Actor {

    [ExportCategory("Required")]
    [Export]
    public GUI GUI {get; private set;}
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
    
    protected override void SetStats(ActorStats newStats) {
        base.SetStats(newStats);

        // Additional things for player
        WeaponManager.reloadSpeed = newStats.reloadSpeed;
    }
    
    public PlayerClassManager classManager;

    public void Init() {
        _Ready();
        Players = new() { this };

        classManager = new(this);

        Camera.currentCamera.Init(this);
        
        InputController.Init(this);
        
        WeaponManager.Init(this);
        ShieldManager.Init(this);

        GUI.Init(this);

        PlayerDeathHandler playerDeathHandler = new(this);

        DamageableComponent.OnDamaged += playerDeathHandler.DamageFramePause;
        DamageableComponent.OnDeath += playerDeathHandler.OnDeath;

        classManager.SwitchClassFromSave();
    }
}
