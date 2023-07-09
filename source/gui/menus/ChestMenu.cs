using System;
using System.Linq;
using Godot;

public partial class ChestMenu : Control, IMenu {
    public event Action Disable;

    [Export]
    private AnimationPlayer animationPlayer;
    [Export]
    private Control itemStatsOverview;
    [Export]
    private Control ItemPreviews;
    [Export]
    private TextureRect newItemImage;
    [Export]
    private Button switchItemButton;

    public Action<Weapon> OnWeaponReplaced;
    
    private int? selectedIndex = null;
    private int? previewedIndex = null;
    private bool hoveringWithinStatsOverview = false;
    private bool freezeStatOverview = false;
    
    private Player viewer;
    private Weapon currentWeaponToSwitchTo = null;
    
    private ColorRect PreviewPanel(int index) => ItemPreviews.GetChild<ColorRect>(index);
    private TextureRect PreviewImage(int index) => ItemPreviews.GetChild(index).GetChild<TextureRect>(0);
 
    public override void _Ready() {
        switchItemButton.Pressed += () => SetItem(currentWeaponToSwitchTo);

        // Make it so that hovering over the items in the GUI will show the statistics
        var children = ItemPreviews.GetChildren();
        
        foreach (ColorRect child in ItemPreviews.GetChildren().Cast<ColorRect>()) {
            child.MouseEntered += () => previewedIndex = int.Parse(child.Name) - 1;
            child.MouseExited += () => previewedIndex = null;
        }
        
        itemStatsOverview.MouseEntered += () => hoveringWithinStatsOverview = true;
        itemStatsOverview.MouseExited += () => hoveringWithinStatsOverview = false;
    }


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


    public void Enable(Player player) {
        // reset values
        hoveringWithinStatsOverview = false;
        selectedIndex = null;
        previewedIndex = null;

        viewer = player;
        Visible = true;

        player.InputController.LeftClicked += FreezeStatOverview;

        if (Disable is not null) {
            foreach (Delegate func in Disable.GetInvocationList()) {
                Disable -= func as Action;
            }
        }
    }
    private void FreezeStatOverview() {
        if (previewedIndex != null) {
            selectedIndex = previewedIndex;
            freezeStatOverview = true;
        }
    }

    private void SetItem(Weapon newWeapon) {

        OnWeaponReplaced?.Invoke(viewer.GetWeapon(selectedIndex ?? -1));

        viewer.SetWeapon(newWeapon, selectedIndex ?? -1);
        
        Disable?.Invoke();
    }

    public void SetItems(Weapon newWeapon) {
        currentWeaponToSwitchTo = newWeapon;

        newItemImage.Texture = newWeapon.Sprite.Texture;
        
        for (int i = 0; i < 3; i++) {
            if (viewer.GetWeapon(i) is null) continue;
            PreviewImage(i).Texture = viewer.GetWeapon(i).Sprite.Texture;
        } 
    }

    public void Switch() => Visible = false;
}

