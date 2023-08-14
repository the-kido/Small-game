using Godot;
using System;
using System.Linq;

public partial class DialogueEvent : LevelCriteria {
    public override event Action Finished;
    
    // Change this in the future if you need it, but atm I see no reason
    // as to why you'd need more dialogue information
    static readonly DialogueInfo dialogueInfo = new();
    
    ConversationController dialoguePlayer => Player.Players[0].GUI.DialoguePlayer;
    public override void Start() {
        ConversationItem[] dialogueLines = GetChildren().Cast<ConversationItem>().ToArray();

        dialoguePlayer.Ended += Finish;
        
        // Call this after the event is attached in case the Ended event is called during Start
        // I'll figure out a workaround later
        dialoguePlayer.Start(dialogueLines, dialogueInfo);
    }
    private void Finish() {
        dialoguePlayer.Ended -= Finish;
        Finished?.Invoke();
    }
} 