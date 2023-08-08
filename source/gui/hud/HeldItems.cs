using Godot;
using System;

public partial class HeldItems : Control {
    
    private Player player;
    public void Init(Player player) {
        this.player = player;

        player.WeaponManager.HeldWeaponChanged += UpdateWeaponDisplays; 

        UpdateWeaponDisplays(player.WeaponManager.GetWeapon(0), 0);
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
        GetChild<Button>(index).GetChild<TextureRect>(0).Texture = weapon.Sprite.Texture;
    
}