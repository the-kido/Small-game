using Godot;
using System;
using System.Linq;

public partial class ConversationEvent : LevelCriteria {
    public override event Action Finished;
    
    // Change this in the future if you need it, but atm I see no reason
    // as to why you'd need more dialogue information
    static readonly ConversationInfo dialogueInfo = new();
    
    ConversationController conversationController => Player.Players[0].GUI.DialoguePlayer;
    public override void Start() {
        ConversationItem[] conversationLines = GetChildren().Cast<ConversationItem>().ToArray();

        conversationController.Ended += Finish;
        
        // Call this after the event is attached in case the Ended event is called during Start
        // I'll figure out a workaround later
        conversationController.Start(conversationLines, dialogueInfo);
    }
    private void Finish() {
        conversationController.Ended -= Finish;
        Finished?.Invoke();
    }
} 