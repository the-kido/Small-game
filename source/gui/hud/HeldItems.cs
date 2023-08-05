using Godot;
using System;

public partial class HeldItems : Control {
    
    private Player player;
    public void Init(Player player) {
        this.player = player;

        player.WeaponManager.HeldWeaponChanged += UpdateWeaponDisplays; 

        UpdateWeaponDisplays(player.WeaponManager.Weapons[0], 0);
        ButtonSetup();
    } 

    private void ButtonSetup() {
        foreach (Button button in GetChildren()) {
            int buttonName = int.Parse(button.Name);
            button.Pressed += () => SetWeapon(buttonName - 1);
        }
    }

    void SetWeapon(int index) {
        GD.Print(player.WeaponManager.Weapons[index]);
        player.WeaponManager.SwitchWeapon(index);
    }
        
    private void UpdateWeaponDisplays(Weapon weapon, int index) {
        GetChild<Button>(index).GetChild<TextureRect>(0).Texture = weapon.Sprite.Texture;

        // for (int i = 0; i < player.WeaponManager.Weapons.Length; i++) {
        //     Weapon weapon = player.WeaponManager.Weapons[i];
            
        //     if (weapon is null) continue;

        //     GetChild<Button>(i).GetChild<TextureRect>(0).Texture = weapon.Sprite.Texture;
        // }
    }
}