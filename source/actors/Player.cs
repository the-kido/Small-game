using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public sealed partial class Player : Actor
{
    // Public fields
    [Export]
    public GUI GUI {get; private set;}
    public List<Actor> NearbyEnemies {get; private set;} = new();

    public InputController InputController;

    #region Held weapons
    public int SelectedWeaponIndex {get; private set;} = 0;
    
    private Weapon[] weapons = new Weapon[3];

    public Weapon GetWeapon(int index) => weapons[index];

    public void SetWeapon(Weapon weapon, int index) {
        weapons[index] = weapon;
        InputController.UpdateHeldWeapon(index);
    }
    #endregion


    // This shouldn't be abused; multiplayer support may (?) happen in the future
    public static List<Player> Players {get; private set;}

    // IDK If this is even staying
    [Export]
    private AudioStreamPlayer2D epicSoundEffectPlayer;

    public override void _Ready() {
        base._Ready();

        weapons[0] = GetNode("Hand").GetChild<Weapon>(0);

        //Default some values
        Players = new() { this };

        GUI.Init(this);
        
        DamageableComponent.OnDamaged += GUI.HealthLable.UpdateHealth;
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
        DamageableComponent.QueueFree();

        GUI.OpenReviveMenu();
    }

    public void DamageFramePause(DamageInstance damageInstance) {
        if (!damageInstance.suppressImpactFrames) PlayImpactFrames(300);
    }

    private async void PlayImpactFrames(int milliseconds) {
        GetTree().Paused = true;
        await Task.Delay(milliseconds);
        GetTree().Paused = false;
    }

    #region signals
    private void OnNearbyEnemyAreaEntered(Node2D body) =>
        NearbyEnemies.Add((Actor) body);
    private void OnNearbyEnemyAreaExited(Node2D body) =>
        NearbyEnemies.Remove((Actor) body);

    #endregion
}
