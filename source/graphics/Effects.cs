using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Game.Graphics;

public static class Effects {

    public static Effect GetEffect(Persistent all) => persistentEffects[all];
    public static Effect GetEffect(Instant all) => instantEffects[all];
    
    public enum Instant {
        Spark,
    }

    public enum Persistent {
        None,
        Fire,
        Wet,
        Gas,
        Shield,
        Ice
    }

    public readonly static Effect
        Fire = new("res://assets/effects/fire.tscn", async (node) => {
            (node as GpuParticles2D).Emitting = false;
            await Task.Delay(1000);
            node.GetChild<GpuParticles2D>(0).Emitting = false;
        }),

        Wet = new("res://assets/effects/wet.tscn"),
        Gas = new("res://assets/effects/gas.tscn"),
        Shield = new("res://assets/effects/shield.tscn"),
        Ice = new("res://assets/effects/ice.tscn", (node) => node.Visible = false);
    
    public readonly static Effect
        Spark = new("res://assets/effects/spark.tscn", true);


    // This has to be below when I instance the effects because otherwise all the values will be null (?!!?!??)
    public static readonly Dictionary<Persistent, Effect> persistentEffects = new() {
        {Persistent.None, new("res://assets/effects/fire.tscn")},
        {Persistent.Fire, Fire},
        {Persistent.Wet, Wet},
        {Persistent.Gas, Gas},
        {Persistent.Shield, Shield},
        {Persistent.Ice, Ice},
    };

    public static readonly Dictionary<Instant, Effect> instantEffects = new() {
        {Instant.Spark, Spark}
    };
}

public class Effect {
    public readonly PackedScene packedScene;
    public readonly Action<Node2D> removeAnimation = (node) => (node as GpuParticles2D).Emitting = false; // Passes the instanced effect and will delete it in some custom way.

    public Effect (string packedScenePath, Action<Node2D> removeAnimation) {
        packedScene = ResourceLoader.Load<PackedScene>(packedScenePath);
        this.removeAnimation = removeAnimation;
    }

    public Effect (string packedScenePath, bool noRemoveAnimation = false) {
        packedScene = ResourceLoader.Load<PackedScene>(packedScenePath);

        if (noRemoveAnimation) removeAnimation = (a) => {};
    }
}