using Godot;
using System.Collections.Generic;
using Game.Players.Mechanics;
using Game.Actors;
using Game.Players.Inputs;
using Game.UI;

namespace Game.Players;

public interface PlayerClass {
    public void ClassInit(Player player);
    public void ClassRemoved(Player player);
    public PlayerClassResource PlayerClassResource {get;}
}

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
    
    public PlayerClass playerClass = PlayerClasses.weird;
    
    public void SetClass(PlayerClass newClass) {
        playerClass?.ClassRemoved(this);

        playerClass = newClass;
        playerClass.ClassInit(this);
        playerClass.PlayerClassResource.DoSafetyChecks(); // Do safety checks first because I can't do this any better.
    }
    
    private void LoadClass() {
        // do some saving stuff here.
        SetClass(PlayerClasses.weird);
    }

    public void Init() {
        _Ready();
        Players = new() { this };

        UpdateSpritesFromResource(playerClass.PlayerClassResource);
        
        Camera.currentCamera.Init(this);
        
        InputController.Init(this);
        
        WeaponManager.Init(this);
        ShieldManager.Init(this);

        GUI.Init(this);

        PlayerDeathHandler playerDeathHandler = new(this);

        DamageableComponent.OnDamaged += playerDeathHandler.DamageFramePause;
        DamageableComponent.OnDeath += playerDeathHandler.OnDeath;

        LoadClass();
    }

    private void UpdateSpritesFromResource(PlayerClassResource playerClassResource) =>
        sprite.SpriteFrames = playerClassResource.playerSprites;
}
