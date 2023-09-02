using Godot;
using System.Linq;
using Game.Players;
using Game.Characters;

namespace Game.LevelContent;

public partial class DialogueInteractable : Node2D {
    [Export]
    Interactable interactable;

    [Export]
    private string dialogueName;
    [Export]
    bool showPortraitImage;
    
    [Export]
    Godot.Collections.Array<ConversationItem> dialogueLines;
    
    bool ignoreSpam;

    ConversationInfo DialogueInfo => new(true, showPortraitImage);

    public override void _Ready() {
        interactable.Interacted += OnInteracted;
    }

    private void OnInteracted(Player player) {
        if (ignoreSpam) return;
        ignoreSpam = true;

        interactable.SetIndicatorVisibility(player, false);
        
        player.GUI.DialoguePlayer.Start(dialogueLines.ToArray(), DialogueInfo);

        player.GUI.DialoguePlayer.Ended += () => IgnoreSpam(player); 
	}

    private void IgnoreSpam(Player player) {
        ignoreSpam = false;
        interactable.SetIndicatorVisibility(player, true);
        player.GUI.DialoguePlayer.Ended -= () => IgnoreSpam(player);
    }
}