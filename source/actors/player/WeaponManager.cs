using Godot;
using System;

// "hand"
public partial class WeaponManager : Node2D{

    public int SelectedSlot {get; private set;} = 0;
    // This is for the visuals
    public Action<Weapon, int> HeldWeaponChanged;
    // This is for the reloadbar thing
    public event Action<Weapon> WeaponSwitched;
    
    [Export]
    private ReloadVisual reloadVisual;
    private Player player;
    public readonly Weapon[] Weapons = new Weapon[3];

    public Weapon GetWeapon(int index) => Weapons[index];
    public Weapon HeldWeapon => Weapons[SelectedSlot];

    public void Init(Player player) {
        this.player = player;
        
        Weapons[0] = GetChild<Weapon>(0);
        HeldWeapon.Enable(true);

        reloadVisual.Init(this);
    }
    
    // Hide the one being used RN and switch to another one
    public void SwitchHeldWeapon(int slot) {
        if (Weapons[slot] is null) return;
        
        Weapons[slot].Enable(true);
        WeaponSwitched?.Invoke(Weapons[slot]);
        HeldWeaponChanged?.Invoke(Weapons[slot], slot);
        
        if (slot == SelectedSlot) return;

        Weapons[SelectedSlot].Enable(false);
        SelectedSlot = slot;
	}

    private void RemoveWeapon(int slot) {
        Weapons[slot]?.Enable(false);
        Weapons[slot]?.QueueFree();
        Weapons[slot] = null;
    }

    public void AddWeapon(Weapon newWeapon, int slot) {
        newWeapon.Name = $"{newWeapon.Name}: {slot}";
        RemoveWeapon(slot);
		// Add the new weapon
        Weapons[slot] = newWeapon;
		AddChild(newWeapon);

        // Just for funzies also switch to the new weapon
        SwitchHeldWeapon(slot);
    }
}