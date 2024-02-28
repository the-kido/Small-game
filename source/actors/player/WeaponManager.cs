using Godot;
using System;
using Game.UI;
using Game.Actors;
using Game.Data;
using Game.Players.Inputs;
using System.Linq;
using Game.SealedContent;

namespace Game.Players.Mechanics;

public partial class WeaponManager : Node2D { // Also called "hand"
    public event Action<Weapon, int> HeldWeaponChanged; // This is for the visuals
    public event Action<Weapon> WeaponSwitched; // This is for the reloadbar thing
    public int SelectedSlot {get; private set;} = 0;
    public ModifiedStat reloadSpeed = new();
    public Weapon[] Weapons {get; private set;} = new Weapon[3];
    public WeaponController WeaponController {get; private set;}

    [Export]
    private ReloadVisual reloadVisual;
    
    public Weapon GetWeapon(int index) => Weapons[index];
    public Weapon HeldWeapon => Weapons[SelectedSlot];

    // Stuff related to saving
    public DataSaver dataSaver;
    private string[] SavedWeapons => 
        Weapons.Select(weapon => weapon?.PackedScene.ResourcePath).ToArray();   
    // Save more at walmart

    public void Init(Player player) {
        player.InputController.WeaponController = new(this, player);
        WeaponController = player.InputController.WeaponController;
        reloadVisual.Init(this);

        dataSaver = new("Weapons", () => SavedWeapons, () => SetToDefaultWeapon(new Normal()));

        Load(player);

        player.classManager.ClassSwitched += SetToDefaultWeapon; 
    }
    
    public void AddAndSwitchWeapon(Weapon newWeapon, int slot) {
        AddWeapon(newWeapon, slot);
        SwitchHeldWeapon(slot);
    }

    public void SwitchHeldWeapon(int slot) {
        if (Weapons[slot] is null)
            return;

        if (Weapons[SelectedSlot] is not null)
            Weapons[SelectedSlot].Visible = false; // Hide old one
        
        Weapons[slot].Visible = true; // Show new one
        
        EnableWeapon(slot);
        WeaponSwitched?.Invoke(Weapons[slot]);
        HeldWeaponChanged?.Invoke(Weapons[slot], slot);
        
        SelectedSlot = slot;
	}

    private void SetToDefaultWeapon(IPlayerClass playerClass) {
        for (int i = 0; i < Weapons.Length; i++) {
            if (Weapons[i] is not null)
                RemoveWeapon(i);
        }
        AddAndSwitchWeapon(playerClass.classResource.defaultWeapon.Instantiate<Weapon>(), 0);    
        GameDataService.Save();
    }

    private void LoadWeapons() {
        var list = (string[]) dataSaver.LoadValue();

        for (int i = 0; i < list.Length; i++) {

            if (string.IsNullOrEmpty(list[i])) 
                continue;
            
            Weapon weapon = ResourceLoader.Load<PackedScene>(list[i]).Instantiate<Weapon>();
            AddWeapon(weapon, i);
        }
    }

    private void Load(Player player) {
        LoadWeapons(); // Load weapons from save.

        if (Weapons[0] is null) // Nothing was loaded. Default to the class's weapon
            SetToDefaultWeapon(player.classManager.playerClass);
        
        SwitchHeldWeapon(0); // Always use the first weapon
    }

    private void EnableWeapon(int slot) {
        WeaponController.UnsubEvents();
        Weapons[slot].AttachEvents(); 
    }

    private void RemoveWeapon(int slot) {
        Weapons[slot]?.QueueFree();
        Weapons[slot] = null;

        HeldWeaponChanged?.Invoke(Weapons[slot], slot);
    }

    private void AddWeapon(Weapon newWeapon, int slot) {
        RemoveWeapon(slot); // Remove whatever is currently there.
        
        newWeapon.Init(this, slot);
        
		AddChild(newWeapon);
		
        Weapons[slot] = newWeapon;
    }
}