using Godot;

public partial class DialogueInteractable : Interactable {

    [Export]
    private string dialogueName;
    [Export]
    bool showPortraitImage;
    
    bool ignoreSpam;

    DialogueInfo dialogueInfo => new(true, showPortraitImage);

    protected override void OnInteracted(Player player) {
        if (ignoreSpam) return;
        ignoreSpam = true;

        SetIndicatorVisibility(player, false);
        
        player.GUI.DialoguePlayer.Start(DialogueLines.Lines[dialogueName], dialogueInfo);

        player.GUI.DialoguePlayer.DialogueEnded += () => IgnoreSpam(player); 
	}

    private void IgnoreSpam(Player player) {
        ignoreSpam = false;
        SetIndicatorVisibility(player, true);
        player.GUI.DialoguePlayer.DialogueEnded -= () => IgnoreSpam(player);
    }
}