using System;
using Godot;
using System.Collections.Generic;

public partial class ChestMenu : Control, IMenu {
    public event Action Disable;

    [Export]
    private AnimationPlayer animationPlayer;
    [Export]
    private Control itemStatsOverview;
    [Export]
    Control ItemPreviews;
    [Export]
    TextureRect newItemImage;

    [Export]
    Button switchItemButton;
    
    ColorRect PreviewPanel(int index) => ItemPreviews.GetChild<ColorRect>(index);
    TextureRect PreviewImage(int index) => ItemPreviews.GetChild(index).GetChild<TextureRect>(0);
    public override void _Ready() {
        
        // Make it so that hovering over the items in the GUI will show the statistics
        var children = ItemPreviews.GetChildren();
        
        for (int i = 0; i < children.Count; i++) {
            
            ColorRect child = (ColorRect) ItemPreviews.FindChild((i+1).ToString());

            child.MouseEntered += () => previewedIndex = int.Parse(child.Name) - 1;
            child.MouseExited += () => previewedIndex = -1;
        }

        itemStatsOverview.MouseEntered += () => hoveringWithinStatsOverview = true;
        itemStatsOverview.MouseExited += () => hoveringWithinStatsOverview = false;
    }


    bool hoveringWithinStatsOverview = false;
    int selectedIndex = -1;
    int previewedIndex = -1;
    // When ready, i want to attach the "mouse entered 
    public override void _Process(double delta) {
        
        GD.Print("Sel:",selectedIndex, "preview:", previewedIndex);

        if (freezeStatOverview) return;
        if (hoveringWithinStatsOverview) return;

        if (previewedIndex != -1) {
            itemStatsOverview.Visible = true;
            itemStatsOverview.GlobalPosition = GetGlobalMousePosition();
        } else {
            itemStatsOverview.Visible = false;
        }
    }


    Player player;
    public void Enable(Player player) {
        this.player = player;
        Visible = true;

        player.InputController.LeftClicked += FreezeStatOverview;

        if (Disable is not null) {
            foreach (Delegate func in Disable.GetInvocationList()) {
                Disable -= func as Action;
            }
        }
    }
    bool freezeStatOverview = false;
    private void FreezeStatOverview() {
        
        if (previewedIndex != -1) {
            selectedIndex = previewedIndex;
            freezeStatOverview = true;
        }else {
            freezeStatOverview = false;
        }
    }

    public Action<Weapon> OnWeaponReplaced;

    private void SetItem(Weapon newWeapon) {
        
        OnWeaponReplaced?.Invoke(player.GetWeapon(selectedIndex));

        player.SetWeapon(newWeapon, selectedIndex);
        Disable?.Invoke();
    }


    // make this not use weapon; it's gotta change to something else.
    public void SetItems(Weapon newWeapon) {
        switchItemButton.Pressed += () => SetItem(newWeapon);

        newItemImage.Texture = newWeapon.Sprite.Texture;
        
        for (int i = 0; i < 3; i++) {
            if (player.GetWeapon(i) is null) continue;
            PreviewImage(i).Texture = player.GetWeapon(i).Sprite.Texture;
        } 
    }

    public void Switch() {

        Visible = false;

    }
}

