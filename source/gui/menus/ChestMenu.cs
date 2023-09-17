using System;
using System.Linq;
using Godot;
using Game.Players;

// I want there to be 1 universal "chest menu" class
// Whatever happens to certain buttons should be customizable 
// Also, different enum type will decide if there are 3 slots (weapons) or 1 slot (shield)

namespace Game.UI;

public partial class ChestMenu : Control, IMenu {
    public event Action Disable;
    public ChestMenu() {
        Disable += () => OnSelectionMade = null;
    }

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
    
    /// <summary>
    /// The int passed by this event is the index that was selected by the player
    /// </summary>
    public event Action<int> OnSelectionMade;
    
    private int? selectedIndex = null;
    private int? previewedIndex = null;
    private bool hoveringWithinStatsOverview = false;
    private bool freezeStatOverview = false;
    
    private Player viewer;
    
    private ColorRect PreviewPanel(int index) => itemPreviews.GetChild<ColorRect>(index);
    private TextureRect PreviewImage(int index) => itemPreviews.GetChild(index).GetChild<TextureRect>(0);
 
    public override void _Ready() {
        closeButton.Pressed += () => Disable?.Invoke();
        switchItemButton.Pressed += SelectionMade;

        // Make it so that hovering over the items in the GUI will show the statistics
        var children = itemPreviews.GetChildren();
        
        foreach (ColorRect child in itemPreviews.GetChildren().Cast<ColorRect>()) {
            child.MouseEntered += () => previewedIndex = int.Parse(child.Name) - 1;
            child.MouseExited += () => previewedIndex = null;
        }
        
        itemOverview.MouseEntered += () => hoveringWithinStatsOverview = true;
        itemOverview.MouseExited += () => hoveringWithinStatsOverview = false;
    }

    private void SelectionMade() {
        OnSelectionMade?.Invoke(selectedIndex ?? -1);
        Disable?.Invoke();
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
            UpdateItemPreview();
            itemOverview.GlobalPosition = GetGlobalMousePosition();
        }
        else {
            itemOverview.Visible = false;
        }
    }

    private void UpdateItemPreview() {
        switch (inspectedItem.Type) {
            case ChestItemType.WEAPON:
                if (viewer.WeaponManager.GetWeapon(previewedIndex ?? 0) is not null) {
                    itemOverview.Visible = true;
                    itemDecription.Text = viewer.WeaponManager.GetWeapon(previewedIndex ?? 0).Description;
                }
                
                break;
            case ChestItemType.SHIELD:
                if (viewer.ShieldManager.HeldShield is not null) {
                    itemOverview.Visible = true;
                    itemDecription.Text = viewer.ShieldManager.HeldShield.Description;
                }
                break;
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

        // Hide all of the preview images such that a selected few will change later
        for (int i = 0; i < 3; i++)
            PreviewPanel(i).Visible = false;
        
        Disable = null;
        OnSelectionMade = null;
    }
    
    private void FreezeStatOverview() {
        if (previewedIndex != null) {
            selectedIndex = previewedIndex;
            freezeStatOverview = true;
        }
    }
    
    IChestItem inspectedItem;
    public void SetItems(IChestItem newItem) {
        inspectedItem = newItem;

        newItemImage.Texture = newItem.Icon;
        newItemDescription.Text = newItem.Description;

        switch (newItem.Type) {
            case ChestItemType.WEAPON:
                for (int i = 0; i < 3; i++) {
                    PreviewPanel(i).Visible = true;
                    if (viewer.WeaponManager.GetWeapon(i) is null) continue;
                    PreviewImage(i).Texture = ((IChestItem)viewer.WeaponManager.GetWeapon(i)).Icon;
                }
                break;
            case ChestItemType.SHIELD:
                PreviewPanel(1).Visible = true;
                PreviewImage(1).Texture = ((IChestItem)viewer.ShieldManager?.HeldShield).Icon;
                break;
        }
    }
    
    public void Close() {
        Visible = false;
        
    }
}

