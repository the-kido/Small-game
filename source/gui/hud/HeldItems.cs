using Godot;
using Game.Players;

namespace Game.UI;

public partial class HeldItems : Control {
    
    private Player player;
    public void Init(Player player) {
        this.player = player;

        if (player.WeaponManager is null) 
            return;

        player.WeaponManager.HeldWeaponChanged += UpdateWeaponDisplays;

        for (int i = 0; i < player.WeaponManager.Weapons.Length; i++) {
            Weapon weapon = player.WeaponManager.Weapons[i];
            
            if (weapon is null) 
                continue;
            
            UpdateWeaponDisplays(weapon, i);
        }
    
        ButtonSetup();
    } 

    private void ButtonSetup() {
        foreach (Button button in GetChildren()) {
            // Check if there is even a weapon in that slot
            
            int buttonName = int.Parse(button.Name);
            button.Pressed += () => SetWeapon(buttonName - 1);
        }
    }

    void SetWeapon(int index) {
        player.WeaponManager.SwitchHeldWeapon(index);
    }
        
    private void UpdateWeaponDisplays(Weapon weapon, int index) =>
        GetChild<Button>(index).GetChild<TextureRect>(0).Texture = weapon.Icon;
}