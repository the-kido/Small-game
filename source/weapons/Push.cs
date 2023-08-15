using System;
using Godot;
using System.Collections.Generic;

public sealed partial class Push : Weapon {
    public override string Description => @$"
{"Push".Colored(Colors.LEGENDARY_RARITY)}
Pushes things, but deals no damage    
    ";
    public override PackedScene PackedScene => throw new NotImplementedException();
    public override Type WeaponType { get; protected set;} = Type.HoldToCharge;

    [Export]
    private Area2D reachArea;

    private Vector2 DegreeAsVector() {
        float rotation = (Hand.RotationDegrees + 90) * MathF.PI /180;
        
        return new(MathF.Sin(rotation), -MathF.Cos(rotation));
    }

    private Node2D[] GetEntitiesToPush() {
        List<Node2D> entitiesToPush = new();
        
        Godot.Collections.Array<Node2D> overlappingBodies = reachArea.GetOverlappingBodies();
        
        foreach (Node2D body in overlappingBodies) {
            Vector2 difference = (body.GlobalPosition - Hand.GlobalPosition).Normalized();
            float distance = DegreeAsVector().DistanceTo(difference);
            if (distance < 0.75f) entitiesToPush.Add(body);
        }

        return entitiesToPush.ToArray();
    }
    public override void Attack() {
        entities = GetEntitiesToPush();
        x = 0;
    }
    
    Node2D[] entities;
    float strength = 300;
    double x = 0;

    private void UpdateEntityVelocities(double delta) {
        if (entities is null) return;
        if (x > 2) return;
        x += delta;

        float y = strength * MathF.Pow(MathF.E, -2 * (float) x); 
        if (y < 10) y = 0;


        foreach (CharacterBody2D entity in entities) {
            entity.Velocity = DegreeAsVector() * y;
        }
        
    }

    public override void _Process(double delta) {
        reloadTimer += delta;
        UpdateEntityVelocities(delta);
    }

    public override void UpdateWeapon(Vector2 attackDirection) {
        Hand.LookAt(attackDirection);
    }

    // TODO; This is exactly like the code in "weirdgun" so idk how to fricking fix that but whatver.
    protected override void OnWeaponUsing(double delta) {
        if (reloadTimer >= ReloadSpeed) {
			reloadTimer = 0;
            Attack();
        }
    }
} 