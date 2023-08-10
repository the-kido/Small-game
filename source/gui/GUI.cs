using Godot;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public partial class GUI : CanvasLayer {

    private Player player;
    public void Init(Player player) {
        this.player = player;
        HealthLable.Init(player);
        HeldItems.Init(player);
        ShieldInfo.Init(player);
    }

    [Export]
    public SelectedTargetIndicator TargetIndicator {get; private set;}

    #region HUD
    [Export]
    public HUD HUD {get; private set;}

    // Make easier access to important member fields of HUD above for encapsulation purposes
    public ToggleAttackButton AttackButton => HUD.AttackButton;
    public DialogueBar DialogueBar => HUD.dialogueBar;
    public DialoguePlayer DialoguePlayer => HUD.dialogueBar.DialoguePlayer;
    public HealthLabelTemp HealthLable => HUD.healthLable;
    public CoinsLabel CoinsLable => HUD.coinsLabel;
    public InteractButton InteractButton => HUD.interactButton;
    public HeldItems HeldItems => HUD.heldItems;
    public ShieldInfo ShieldInfo => HUD.shieldInfo;



    private void CoverHUD(bool cover) => HUD.Cover(cover);


    #endregion
    
    #region All Menus
    [Export]
    private ReviveMenu reviveMenu;
    [Export]
    public ChestMenu chestMenu;

    #endregion

    #region Menu open methods
    public void OpenReviveMenu() => SetCurrentMenu(reviveMenu);

    public void OpenChestMenu(IChestItem newWeapon) {
        SetCurrentMenu(chestMenu);
        chestMenu.SetItems(newWeapon);
    }

    #endregion
    public IMenu CurrentMenu {get; private set;}
    
    private void CloseCurrentMenu() {
        CoverHUD(false);
        CurrentMenu?.Switch();
        CurrentMenu = null;
    }

    private void SetCurrentMenu(IMenu newMenu) {
        CoverHUD(true);

        CurrentMenu?.Switch();

        CurrentMenu = newMenu;
        
        CurrentMenu?.Enable(player);
        
        CurrentMenu.Disable += () => {
            CloseCurrentMenu();
        };
    }
}


