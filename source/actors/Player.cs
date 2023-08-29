using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Players.Mechanics;
using Game.Actors;
using Game.Players.Inputs;
using Game.UI;
using Game.Damage;

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
    
    protected override void UpdateStats(ActorStats newStats) {
        base.UpdateStats(newStats);
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
        
        // Init required components
        WeaponManager.Init(this);
        ShieldManager.Init(this, playerClass.PlayerClassResource);

        GUI.Init(this);

        DamageableComponent.OnDamaged += DamageFramePause;
        DamageableComponent.OnDeath += OnDeath;

        LoadClass();
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
