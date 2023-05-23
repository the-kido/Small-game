using Godot;
using System;
using System.Threading.Tasks;

public partial class GUI : CanvasLayer {
     
    [Export]
    public PlayerTargetIndicator TargetIndicator {get; private set;}
    
    [Export]
    public HUD HUD;

    public Player ConnectedPlayer {get; set;}
    
    #region All Menus
    [Export]
    public ReviveMenu ReviveMenu {get; private set;}
    #endregion

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

    public override void _Ready() {
        
    }
}


