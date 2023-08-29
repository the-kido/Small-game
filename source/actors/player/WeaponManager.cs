using Godot;
using System;
using Game.UI;
using Game.Actors;
using Game.Data;
using Game.Players.Inputs;

namespace Game.Players.Mechanics;

public partial class WeaponManager : Node2D, ISaveable { // Also called "hand"
    public int SelectedSlot {get; private set;} = 0;
    public Action<Weapon, int> HeldWeaponChanged; // This is for the visuals
    public event Action<Weapon> WeaponSwitched; // This is for the reloadbar thing
    public ModifiedStat reloadSpeed = new();

    public WeaponController WeaponController {get; private set;}

    [Export]
    private ReloadVisual reloadVisual;
    public readonly Weapon[] Weapons = new Weapon[3];

    public Weapon GetWeapon(int index) => Weapons[index];
    public Weapon HeldWeapon => Weapons[SelectedSlot];

    // Stuff related to saving
    public SaveData SaveData => new("Weapons", SavedWeapons);
    public static string[] SavedWeapons {get; private set;} = new string[3]; 
    // Save more at walmart 

    private void OnClassSwitched(PlayerClass playerClass) {
        RemoveSavedWeapons();
        AddWeapon(playerClass.PlayerClassResource.defaultWeapon.Instantiate<Weapon>(), 0);
    }

    private void LoadWeapons() {
        var list = (Godot.Collections.Array<string>) (this as ISaveable).LoadData();
        
        for (int i = 0; i < list.Count; i++) {

            if (string.IsNullOrEmpty(list[i])) 
                continue;
            
            Weapon weapon = ResourceLoader.Load<PackedScene>(list[i]).Instantiate<Weapon>();
            AddWeapon(weapon, i);
        }
    }

    private static void RemoveSavedWeapons() {
        SavedWeapons = new string[3];
        GameDataService.Save();
    }


    public void Init(Player player) {
        PlayerManager.ClassSwitched += OnClassSwitched; 
        
        player.InputController.WeaponController = new(this, player);
        WeaponController = player.InputController.WeaponController;

        (this as ISaveable).InitSaveable();
        
        LoadWeapons();
        
        AddWeapon(player.playerClass.PlayerClassResource.defaultWeapon.Instantiate<Weapon>(), 0);

        reloadVisual.Init(this);

        player.DamageableComponent.OnDeath += (_) => RemoveSavedWeapons();
    }
    
    public void SwitchHeldWeapon(int slot) {
        if (Weapons[slot] is null) 
            return;
        
        Weapons[slot].Enable(true);
        WeaponSwitched?.Invoke(Weapons[slot]);
        HeldWeaponChanged?.Invoke(Weapons[slot], slot);
        
        if (slot == SelectedSlot) 
            return;

        Weapons[SelectedSlot].Enable(false);
        SelectedSlot = slot;
	}

    private void RemoveWeapon(int slot) {
        Weapons[slot]?.Enable(false);
        Weapons[slot]?.QueueFree();
        Weapons[slot] = null;
        SavedWeapons[slot] = null;
    }

    public void AddWeapon(Weapon newWeapon, int slot) {
        newWeapon.Name = $"{newWeapon.Name}: {slot}";

        RemoveWeapon(slot);
        
		// Add the new weapon
        Weapons[slot] = newWeapon;
        SavedWeapons[slot] = newWeapon.PackedScene.ResourcePath;

		AddChild(newWeapon);

        newWeapon.Init(this);

        // Just for funzies also switch to the new weapon
        SwitchHeldWeapon(slot);
    }
}