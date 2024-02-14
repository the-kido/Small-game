using System;
using Godot;
using System.Collections.Generic;
using Game.Actors;

namespace Game.SealedContent;

public sealed partial class Push : Weapon {
    public override string Description => @$"
{"Push".Colored(Colors.LEGENDARY_RARITY)}
Pushes things, but deals no damage    
    ";
    public override PackedScene PackedScene => ResourceLoader.Load<PackedScene>("res://source/weapons/push.tscn");
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
        
        ToggleAI(true);
        
        direction = DegreeAsVector();
        x = 0;
    }
    
    private void ToggleAI(bool @bool) {
        foreach (var entity in entities) {
            if (entity is Enemy enemy) {
                enemy.PauseAI = @bool;
            }
        }
    }

    Node2D[] entities;
    const float STRENGTH = 400;
    const float TIME = 2.5f; 
    double x = 0;

    Vector2 previous;
    Vector2 direction;
    private void UpdateEntityVelocities(double delta) {
        if (entities is null) return;
        
        x += delta;
        
        if (x > TIME) return;

        if (x + delta > TIME) ToggleAI(false);

        //float y = strength * MathF.Pow(MathF.E, -2 * (float) x); 
        float y = Math.Max(100 * MathF.Log((float) -x + TIME) + STRENGTH, 0); 

        foreach (Actor entity in entities) {
            // This is in case the entity dies to the void. We don't wanna play with the velocity anymore if that's the case.
            if (!entity.DamageableComponent.IsAlive) return;
            
            entity.Velocity += (direction * y) - previous;
            previous = direction * y;
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
        if (reloadTimer >= EffectiveReloadSpeed) {
			reloadTimer = 0;
            Attack();
        }
    }
} 