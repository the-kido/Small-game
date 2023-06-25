using Godot;

public partial class DialogueInteractable : Interactable {

    [Export]
    private string dialogueName;

    bool ignoreSpam;
    protected override void OnInteracted(Player player) {
        if (ignoreSpam) return;
        ignoreSpam = true;
        
        player.GUI.DialoguePlayer.Start(DialogueLines.Lines[dialogueName], new());

        player.GUI.DialoguePlayer.DialogueEnded += () => IgnoreSpam(player); 
	}

    private void IgnoreSpam(Player player) {
        ignoreSpam = false;
        player.GUI.DialoguePlayer.DialogueEnded -= () => IgnoreSpam(player);
    }
}