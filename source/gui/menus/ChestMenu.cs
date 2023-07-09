using System;
using Godot;


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
        switchItemButton.Pressed += () => SetItem(currentWeaponToSwitchTo);

        // Make it so that hovering over the items in the GUI will show the statistics
        var children = ItemPreviews.GetChildren();
        
        foreach (ColorRect child in ItemPreviews.GetChildren()) {
            
            child.MouseEntered += () => previewedIndex = int.Parse(child.Name) - 1;
            child.MouseExited += () => previewedIndex = null;
        }
        
        itemStatsOverview.MouseEntered += () => hoveringWithinStatsOverview = true;
        itemStatsOverview.MouseExited += () => hoveringWithinStatsOverview = false;
    }


    bool hoveringWithinStatsOverview = false;

    int? selectedIndex = null;
    int? previewedIndex = null;
    // When ready, i want to attach the "mouse entered 
    public override void _Process(double delta) {

        if (freezeStatOverview) {
            itemStatsOverview.MouseFilter = MouseFilterEnum.Stop;
            if (!hoveringWithinStatsOverview && previewedIndex is null) freezeStatOverview = false;
            else return;
        }
        else{
            itemStatsOverview.MouseFilter = MouseFilterEnum.Ignore;
        }
        if (hoveringWithinStatsOverview) return;

        if (previewedIndex != null) {
            itemStatsOverview.Visible = true;
            itemStatsOverview.GlobalPosition = GetGlobalMousePosition();
        } else {
            itemStatsOverview.Visible = false;
        }
    }


    Player player;
    public void Enable(Player player) {
        // reset values
        hoveringWithinStatsOverview = false;
        selectedIndex = null;
        previewedIndex = null;

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
        if (previewedIndex != null) {
            selectedIndex = previewedIndex;
            freezeStatOverview = true;
        }
    }

    public Action<Weapon> OnWeaponReplaced;

    private void SetItem(Weapon newWeapon) {

        OnWeaponReplaced?.Invoke(player.GetWeapon(selectedIndex ?? -1));

        player.SetWeapon(newWeapon, selectedIndex ?? -1);
        
        Disable?.Invoke();
    }

    Weapon currentWeaponToSwitchTo = null;
    public void SetItems(Weapon newWeapon) {
        currentWeaponToSwitchTo = newWeapon;

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

