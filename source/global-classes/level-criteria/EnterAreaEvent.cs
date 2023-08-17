using System;
using Godot;

namespace Game.LevelContent.Criteria;

[GlobalClass]
public partial class EnterAreaEvent : LevelCriteria {
    [Export]
    private Area2D area;

    public override event Action Finished;

    public override void Start() =>
        area.BodyEntered += OnFinished; 

    private void OnFinished(Node2D _) {
        area.BodyEntered -= OnFinished; 
        Finished?.Invoke();
    }
}
