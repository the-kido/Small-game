using Godot;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public partial class GUI : CanvasLayer {
     

    [Export]
    public SelectedTargetIndicator TargetIndicator {get; private set;}

    #region HUD
    [Export]
    private HUD HUD;
    // Make easier access to important member fields of HUD above for encapsulation purposes
    public ToggleAttackButton AttackButton => HUD.AttackButton;
    public DialogueBar DialogueBar => HUD.dialogueBar;
    public DialoguePlayer DialoguePlayer => HUD.dialogueBar.DialoguePlayer;
    public HealthLabelTemp HealthLable => HUD.healthLable;
    #endregion

    
    #region All Menus
    [Export]
    public ReviveMenu ReviveMenu {get; private set;}
    #endregion

        
    public static List<GUI> PlayerGUIs {get; private set;} = new List<GUI>();

    public IMenu CurrentMenu {get; private set;}
    
    private void CloseCurrentMenu() {
        CurrentMenu?.Switch();
        CurrentMenu = null;
    }
    public void SetCurrentMenu(IMenu newMenu) {
        CurrentMenu?.Switch();


        CurrentMenu = newMenu;
        
        CurrentMenu?.Enable();

        CurrentMenu.Disable += () => {
            CloseCurrentMenu();
        };
    }

    public override void _Ready() => PlayerGUIs.Add(this);
}


