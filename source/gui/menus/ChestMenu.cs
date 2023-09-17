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
    
    // [Export]
    // private Control itemOverview;
    
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
    
    private Player viewer;
    
    private ColorRect PreviewPanel(int index) => itemPreviews.GetChild<ColorRect>(index);
    private TextureRect PreviewImage(int index) => itemPreviews.GetChild(index).GetChild<TextureRect>(0);
 
    public override void _Ready() {
        closeButton.Pressed += () => Disable?.Invoke();

        // Make it so that hovering over the items in the GUI will show the statistics
        var children = itemPreviews.GetChildren();
        
        foreach (HoverButton child in itemPreviews.GetChildren().Cast<HoverButton>()) {
            int index = int.Parse(child.Name) - 1;

            child.SelectionMade += () => SelectionMade(index); 
            child.BeingInspected += () => UpdateItemPreview(index);
        }
    }

    private void SelectionMade(int selectedIndex) {
        OnSelectionMade?.Invoke(selectedIndex);
        Disable?.Invoke();
    }

    private void UpdateItemPreview(int index) {
        switch (inspectedItem.Type) {
            case ChestItemType.WEAPON:
                if (viewer.WeaponManager.GetWeapon(index) is not null) {
                    itemDecription.Text = viewer.WeaponManager.GetWeapon(index).Description;
                }
                break;
            case ChestItemType.SHIELD:
                if (viewer.ShieldManager.HeldShield is not null) {
                    itemDecription.Text = viewer.ShieldManager.HeldShield.Description;
                }
                break;
        }
    }
    
    public void Enable(Player player) {
        viewer = player;
        Visible = true;

        // Hide all of the preview images such that a selected few will change later
        for (int i = 0; i < 3; i++)
            PreviewPanel(i).Visible = false;

        foreach (HoverButton hoverButton in itemPreviews.GetChildren()) {
            hoverButton.Init(player);
        }
        
        Disable = null;
        OnSelectionMade = null;
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
        foreach (HoverButton child in itemPreviews.GetChildren().Cast<HoverButton>()) {
            child.Close(viewer);
        }
    }
}

