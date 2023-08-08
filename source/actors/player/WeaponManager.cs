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
    private readonly Weapon[] Weapons = new Weapon[3];

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
        Weapons[SelectedSlot].Enable(false);
        Weapons[slot].Enable(true);
        WeaponSwitched?.Invoke(Weapons[slot]);
        
        SelectedSlot = slot;
        
        HeldWeaponChanged?.Invoke(Weapons[slot], slot);
	}

    public void AddWeapon(Weapon newWeapon, int slot) {

        // TEchnically it's similar to the above function
        // But it's hard to make it use the abv function...
        // For reasons.

        // TODO
        // See if I can clean this mess up

        Weapon oldWeapon = Weapons[slot];
        oldWeapon?.Enable(false);

        Weapon newWeaponInstance = newWeapon.PackedScene.Instantiate<Weapon>(); 

        SelectedSlot = slot;

		// Add the new weapon
        Weapons[slot] = newWeaponInstance;
		AddChild(newWeaponInstance);
        newWeaponInstance.Enable(true);

        HeldWeaponChanged?.Invoke(newWeaponInstance, slot);
      
        oldWeapon.Enable(false);

        oldWeapon?.QueueFree();
	}
}