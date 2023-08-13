using Godot;
using System;

public partial class DialogueEvent : LevelCriteria
{
    public override event Action Finished;

    [Export]
    private string dialogue;
    
    // Change this in the future if you need it, but atm I see no reason
    // as to why you'd need more dialogue information
    static readonly DialogueInfo dialogueInfo = new();
    public override void Start() {
        Player.Players[0].GUI.DialoguePlayer.Start(DialogueLines.Lines[dialogue], dialogueInfo);
    }
} 