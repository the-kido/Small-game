using Godot;
using System.Linq;

public partial class DialogueInteractable : Interactable {

    [Export]
    private string dialogueName;
    [Export]
    bool showPortraitImage;
    
    [Export]
    Godot.Collections.Array<ConversationItem> dialogueLines;
    
    bool ignoreSpam;

    ConversationInfo dialogueInfo => new(true, showPortraitImage);

    protected override void OnInteracted(Player player) {
        if (ignoreSpam) return;
        ignoreSpam = true;

        SetIndicatorVisibility(player, false);
        
        player.GUI.DialoguePlayer.Start(dialogueLines.ToArray(), dialogueInfo);

        player.GUI.DialoguePlayer.Ended += () => IgnoreSpam(player); 
	}

    private void IgnoreSpam(Player player) {
        ignoreSpam = false;
        SetIndicatorVisibility(player, true);
        player.GUI.DialoguePlayer.Ended -= () => IgnoreSpam(player);
    }
}