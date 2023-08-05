using Godot;
using System;


public partial class WeaponManager : Node2D{
    
    Player player;

    // currently useless...
    public int SelectedWeaponIndex {get; private set;} = 0;

    public Action<Weapon, int> HeldWeaponChanged;

    public readonly Weapon[] Weapons = new Weapon[3];
    public Weapon GetWeapon(int index) => Weapons[index];

    public Weapon heldWeapon => GetChild<Weapon>(0);

    public void Init(Player player) {
        this.player = player;
        Weapons[0] = GetChild<Weapon>(0);
    }

    public void SwitchWeapon(int index) {
        SetHeldWeapon(index);
        HeldWeaponChanged?.Invoke(Weapons[index], index);
    }
    
    public void SetWeapon(Weapon weapon, int index) {
        Weapons[index] = weapon;
        SetHeldWeapon(index);
        HeldWeaponChanged?.Invoke(Weapons[index], index);
    }

    public void SetHeldWeapon(int slot) {
		Weapon neww = heldWeapon.ChangeWeapon(player.WeaponManager.GetWeapon(slot));
		player.WeaponManager.Weapons[slot] = neww;
	}
	Node2D WeaponStorage => (Node2D) FindChild("Storage");	

    //TODO
	public void SwitchHeldWeapon(int slot) {
		WeaponStorage.AddChild(heldWeapon);
	}

}