using System;
using System.Linq;
using Godot;

public partial class ChestMenu : Control, IMenu {
    public event Action Disable;

    [Export]
    private AnimationPlayer animationPlayer;
    [Export]
    private Control itemOverview;
    [Export] 
    private RichTextLabel itemDecription;
    
    [Export]
    private RichTextLabel newItemDescription;
    [Export]
    private TextureRect newItemImage;
    
    [Export]
    private Control itemPreviews;
    [Export]
    private Button switchItemButton;
    [Export]
    private Button closeButton;


    public Action<Weapon> OnWeaponReplaced;
    
    private int? selectedIndex = null;
    private int? previewedIndex = null;
    private bool hoveringWithinStatsOverview = false;
    private bool freezeStatOverview = false;
    
    private Player viewer;
    private Weapon currentWeaponToSwitchTo = null;
    
    private ColorRect PreviewPanel(int index) => itemPreviews.GetChild<ColorRect>(index);
    private TextureRect PreviewImage(int index) => itemPreviews.GetChild(index).GetChild<TextureRect>(0);
 
    public override void _Ready() {
        closeButton.Pressed += () => Disable?.Invoke();
        switchItemButton.Pressed += () => SetItem(currentWeaponToSwitchTo);

        // Make it so that hovering over the items in the GUI will show the statistics
        var children = itemPreviews.GetChildren();
        
        foreach (ColorRect child in itemPreviews.GetChildren().Cast<ColorRect>()) {
            child.MouseEntered += () => previewedIndex = int.Parse(child.Name) - 1;
            child.MouseExited += () => previewedIndex = null;
        }
        
        itemOverview.MouseEntered += () => hoveringWithinStatsOverview = true;
        itemOverview.MouseExited += () => hoveringWithinStatsOverview = false;
    }


    public override void _Process(double delta) {

        if (freezeStatOverview) {
            itemOverview.MouseFilter = MouseFilterEnum.Stop;
            if (!hoveringWithinStatsOverview && previewedIndex is null) freezeStatOverview = false;
            else return;
        }
        else{
            itemOverview.MouseFilter = MouseFilterEnum.Ignore;
        }
        if (hoveringWithinStatsOverview) return;

        if (previewedIndex != null) {
            
            if (viewer.WeaponManager.GetWeapon(previewedIndex ?? 0) is not null) {
                itemOverview.Visible = true;
                itemDecription.Text = viewer.WeaponManager.GetWeapon(previewedIndex ?? 0).Description;
            }
            
            itemOverview.GlobalPosition = GetGlobalMousePosition();
        } else {
            itemOverview.Visible = false;
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

        OnWeaponReplaced?.Invoke(viewer.WeaponManager.GetWeapon(selectedIndex ?? -1));

        viewer.WeaponManager.AddWeapon(newWeapon, selectedIndex ?? -1);
        
        Disable?.Invoke();
    }

    public void SetItems(Weapon newWeapon) {
        currentWeaponToSwitchTo = newWeapon;

        newItemImage.Texture = newWeapon.Sprite.Texture;
        newItemDescription.Text = newWeapon.Description;
        
        for (int i = 0; i < 3; i++) {
            if (viewer.WeaponManager.GetWeapon(i) is null) continue;
            PreviewImage(i).Texture = viewer.WeaponManager.GetWeapon(i).Sprite.Texture;
        } 
    }

    public void Switch() => Visible = false;
}

