using Godot;

namespace Game.UI;

/// <summary>
/// Handles the GUIs seen DURING the gameplay. Things like health and quests are shown here.
/// </summary>
public partial class HUD : Control {
    [Export]
    public ToggleAttackButton AttackButton {get; private set;}
    [Export]
    public DialogueBar dialogueBar;
    [Export]
    public LevelCompletionIndicator levelCompletionIndicator;
    [Export]
    public HealthLabelTemp healthLable;
    [Export]
    public CoinsLabel coinsLabel;   
    [Export]
    public InteractButton interactButton;
    [Export]
    public HeldItems heldItems;
    [Export]
    public ShieldInfo shieldInfo;
    [Export]
    public InteractableDescription InteractableDescription;
    [Export]
    public LevelEventDescription levelEventDescription;

    [Export]
    public HUDCover HUDCover;
    
    public void Cover(bool enable) => HUDCover.Enable(enable);
}

