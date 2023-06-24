using Godot;

public partial class DialogueInteractable : Interactable {

    [Export]
    private string dialogueName;
    protected override void OnInteracted(Player player) {
        player.GUI.DialoguePlayer.Start(DialogueLines.Lines[dialogueName], new());
	}
}