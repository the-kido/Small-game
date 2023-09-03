using System;
using Godot;
using Game.Players;

namespace Game.UI;

public partial class GUI : CanvasLayer {

    private Player player;
    public void Init(Player player) {

        this.player = player;

        HealthLable.Init(player);
        HeldItems.Init(player);
        ShieldInfo.Init(player);
        LevelCompletionIndicator.Init();

        debugHUD.Init();

        player.DamageableComponent.OnDeath += (_) => OpenReviveMenu();
        player.InputController.PressedEscape += OnEscapePressed;
    }
    // If there's a menu, close it
    // If there's no menu, open the escape menu/
    private void OnEscapePressed() {
        if (CurrentMenu is null)
            OpenEscapeMenu();
        else if (CurrentMenu is not ReviveMenu) // You can't close out of the revive menu with escape..
            CloseCurrentMenu();
    }

    [Export]
    public SelectedTargetIndicator TargetIndicator {get; private set;}

    #region HUD
    [Export]
    public HUD HUD {get; private set;}

    // Make easier access to important member fields of HUD above for encapsulation purposes
    public ToggleAttackButton AttackButton => HUD.AttackButton;
    public DialogueBar DialogueBar => HUD.dialogueBar;
    public LevelCompletionIndicator LevelCompletionIndicator => HUD.levelCompletionIndicator;
    public ConversationController DialoguePlayer => HUD.dialogueBar.ConversationController;
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
    // Menus accessible at spawn
    [Export]
    public PlayerClassMenu playerClassMenu;
    [Export]
    public EscapeMenu escapeMenu;
    [Export]
    public SettingsPage settingsPage;
    
    public enum SpawnMenu {
        PlayerClassMenu = 0,
    }
    
    #endregion

    #region Debug
    [Export] DebugHUD debugHUD;
    #endregion

    #region Menu open methods
    public void OpenReviveMenu() => SetCurrentMenu(reviveMenu);
    private void OpenEscapeMenu() => SetCurrentMenu(escapeMenu);
    public void OpenSettingsPage() => SetCurrentMenu(settingsPage);
    
    public void OpenChestMenu(IChestItem newWeapon) {
        SetCurrentMenu(chestMenu);
        chestMenu.SetItems(newWeapon);
    }
    public void OpenSpawnMenu(SpawnMenu spawnMenu) {
        IMenu selectedMenu = spawnMenu switch {
            SpawnMenu.PlayerClassMenu => playerClassMenu,
            _ => throw new NotImplementedException(),
        };
        SetCurrentMenu(selectedMenu);
    }

    #endregion
    public IMenu CurrentMenu {get; private set;}
    
    private void CloseCurrentMenu() {
        CoverHUD(false);
        CurrentMenu?.Close();
        CurrentMenu = null;
    }

    private void SetCurrentMenu(IMenu newMenu) {
        CoverHUD(true);

        CurrentMenu?.Close();

        CurrentMenu = newMenu;
        
        CurrentMenu?.Enable(player);
        
        CurrentMenu.Disable += CloseCurrentMenu;
    }
}