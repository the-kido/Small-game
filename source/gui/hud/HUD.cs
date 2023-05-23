using Godot;

/// <summary>
/// Handles the GUIs seen DURING the gameplay. Things like health and quests are shown here.
/// </summary>
public partial class HUD : Control {
    [Export]
    public ToggleAttackButton AttackButton {get; private set;}
    [Export]
    public DialogueBar dialogueBar;
    [Export]
    HealthLabelTemp healthLable;
}

