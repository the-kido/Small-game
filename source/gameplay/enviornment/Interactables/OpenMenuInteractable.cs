using System.ComponentModel;
using Game.LevelContent;
using Game.Players;
using Game.UI;
using Godot;

public sealed partial class OpenMenuInteractable : Node2D {
    [Export]
    private Interactable interactable;

    [Export, Description("This path is relative to the GUI scene's root node, which leads to a node with an attached IMenu-inherited class.")]
    private GUI.SpawnMenu menu;

    public override void _Ready() {
        interactable.Interacted += OpenMenu; 
    }

    private void OpenMenu(Player player) {
        player.GUI.OpenSpawnMenu(menu);
    }
}