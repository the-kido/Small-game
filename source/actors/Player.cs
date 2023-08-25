using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Players.Mechanics;
using Game.Actors;
using Game.Players.Inputs;
using Game.UI;
using Game.Damage;
using Game.ActorStatuses;

namespace Game.Players;

public partial class Player : Actor {

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

    public Dictionary<string, Variant> SerializedPlayerExports => new() {
            {"GUI", GUI},
            {"InteractableRadar", InteractableRadar},
            {"InputController", InputController},
            {"WeaponManager", WeaponManager},
            {"ShieldManager", ShieldManager},
            {"epicSoundEffectPlayer", epicSoundEffectPlayer},
            {"Effect", Effect},
            {"DamageableComponent", DamageableComponent},
            {"flippedSprite", sprite},
            {"CollisionShape", CollisionShape},
            {"MoveSpeed", moveSpeed},
        };

    public void SetDataFromSerializedExports(Dictionary<string, Variant> serializedInfo) {
        GUI = (GUI) serializedInfo["GUI"];
        InteractableRadar = (PlayerInteractableRadar) serializedInfo["InteractableRadar"];
        InputController = (InputController) serializedInfo["InputController"];
        WeaponManager = (WeaponManager) serializedInfo["WeaponManager"];
        ShieldManager = (ShieldManager) serializedInfo["ShieldManager"];
        epicSoundEffectPlayer = (AudioStreamPlayer2D) serializedInfo["epicSoundEffectPlayer"];
        Effect = (EffectInflictable) serializedInfo["Effect"];
        DamageableComponent = (Damageable) serializedInfo["DamageableComponent"];
        sprite = (AnimatedSprite2D) serializedInfo["flippedSprite"];
        CollisionShape = (CollisionShape2D) serializedInfo["CollisionShape"];
        moveSpeed = (int) serializedInfo["MoveSpeed"];
    }

    protected override void UpdateStats(ActorStats newStats) {
        base.UpdateStats(newStats);
        // Additional things for player
        WeaponManager.reloadSpeed = newStats.reloadSpeed;
    }
    
    public virtual void ClassInit() {}
    
    public sealed override void _Ready() {}
    
    protected virtual PlayerClassResource PlayerClassResource => 
        ResourceLoader.Load<PlayerClassResource>("res://assets/content/classes/default.tres");

    public void Init() {
        base._Ready();
        ClassInit();
        
        Players = new() { this };

        PlayerClassResource.DoSafetyChecks(); // Do safety checks first because I can't do this any better.
        UpdateSpritesFromResource(PlayerClassResource);
        
        Camera.currentCamera.Init(this);
        
        InputController.Init(this);
        
        // Init required components
        WeaponManager.Init(this, PlayerClassResource);
        ShieldManager.Init(this);

        GUI.Init(this);

        DamageableComponent.OnDamaged += DamageFramePause;
        DamageableComponent.OnDeath += OnDeath;
    }

    private void UpdateSpritesFromResource(PlayerClassResource playerClassResource) =>
        sprite.SpriteFrames = playerClassResource.playerSprites;
    
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
