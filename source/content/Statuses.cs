using System;
using System.Collections.Generic;
using Godot;
using Game.Actors;
using Game.Autoload;
using Game.Damage;
using Game.Graphics;
using Game.Players;

namespace Game.ActorStatuses;


public enum AllStatuses {
    FireEffect,
    WetStatus,
    ShieldedStatus,
    GasStatus,
    PlasmaEffect,
    FreezeEffect,
    
    None = -1,
}

// Just for the list
public abstract partial class ActorStatus {
    public static readonly List<Type> All = new() {
        typeof(FireEffect), typeof(WetStatus), typeof(ShieldedStatus), typeof(GasStatus), typeof(PlasmaEffect), typeof(FreezeEffect) 
    };
}


public sealed class FireEffect : ActorStatus {
    public override float Duration {get; protected set;} = 10;
    public override ConvertsTo[] opposites { get; init;}
    public override string[] incompatibles {get; init;}

    public FireEffect() {
        opposites = new ConvertsTo[] {
            new(typeof(WetStatus), new GasStatus()),
        };

        incompatibles = new string[] {
            nameof(WetStatus),
            nameof(GasStatus)
        };
    }

    private DamageInstance damage;
    
    Node2D fire;
    public override void Init(Actor actor) {
        
        damage = new(actor) {
            damage = 2,
            suppressImpactFrames = true,
        };
        
        // actor.Modulate += new Color(0.3f, 0.2f, 0f);
        fire = ParticleFactory.AddParticle(actor, Effects.Fire);
    }
    public override void Disable(Actor actor) {
        // actor.Modulate -= new Color(0.3f, 0.2f, 0f);

        ParticleFactory.RemoveParticle(fire);
    }

    double damageTime;
    public float damagePeriod = 2;

    public override void Update(Actor actor, double delta) {
        
        damageTime += delta;
        if (damageTime > damagePeriod) {
            damageTime = 0;

            if (actor.DamageableComponent?.ImmuneToDamageFrom?.Contains(AllStatuses.FireEffect) ?? false) 
                return;

            actor.DamageableComponent.Damage(damage);
        }
    }
}

public sealed class WetStatus : PermanentStatus {

    public override ConvertsTo[] opposites {get; init;}
    public override string[] incompatibles {get; init;}

    public WetStatus() {
        opposites = new ConvertsTo[] {
            new ConvertsTo(typeof(FireEffect), new GasStatus()),
        };

        incompatibles = new string[] {
            nameof(FireEffect),
        };
    }

    Node2D water;
    
    ActorStats debuff = new() {
        speed = new(0.1f, 0), // 10 times slower.
        damageDealt = new(1f, -0.2f) 
    }; 
    
    public override void Init(Actor actor) {
        water = ParticleFactory.AddParticle(actor, Effects.Wet);
        actor.StatsManager.AddStats(debuff);
    }

    public override void Disable(Actor actor) {
        actor.StatsManager.RemoveStats(debuff);
        ParticleFactory.RemoveParticle(water);
    }
}

public sealed class ShieldedStatus : PermanentStatus {
    public override ConvertsTo[] opposites {get; init;} = Array.Empty<ConvertsTo>();
    public override string[] incompatibles {get; init;} = Array.Empty<string>();

    ActorStats buff = new() {
        damageTaken = new(0f, -100),
    };

    public override void Disable(Actor actor) {
        ParticleFactory.RemoveParticle(particle);
        actor.StatsManager.RemoveStats(buff);
    }

    Node2D particle;

    public override void Init(Actor actor) {
        actor.StatsManager.AddStats(buff);
        particle = ParticleFactory.AddParticle(actor, Effects.Shield);
    }
}

public sealed class GasStatus : PermanentStatus {
    public override ConvertsTo[] opposites {get; init;}
    public override string[] incompatibles {get; init;}

    public GasStatus() {
        opposites = new ConvertsTo[] {
            
        };
    }
    public override void Disable(Actor actor) {
        ParticleFactory.RemoveParticle(gas);
    }

    Node2D gas;
    //TODO: Just make this have a cute visual. Gas state should do nothing paticularly cool. 
    public override void Init(Actor actor) {    
        gas = ParticleFactory.AddParticle(actor, Effects.Gas);
    }
}

// Fire and electricity ? 
// Or fire and gas ?
public sealed class PlasmaEffect : ActorStatus {
    public override float Duration {get; protected set;} = 10;
    public override ConvertsTo[] opposites { get; init;}
    //use nameof to get the name of the class that it is incompatible with. 
    public override string[] incompatibles {get; init;}

    ActorStats buff = new() {
        damageTaken = new(0, 0.5f),
    };

    public override void Init(Actor actor) { 
        //init the effect too. 
        actor.StatsManager.AddStats(buff);
    }
    public override void Disable(Actor actor) {
        actor.StatsManager.RemoveStats(buff);
    }
    public override void Update(Actor actor, double delta) {}
}

public sealed class FreezeEffect : ActorStatus {
    public override float Duration {get; protected set; } = 5;
    public override ConvertsTo[] opposites {get; init;}
    public override string[] incompatibles { get; init;}

    public FreezeEffect() {
        opposites = new ConvertsTo[] {
            new(typeof(FireEffect), new WetStatus()),
        };
        incompatibles = new string[] {
            nameof(FireEffect),
            nameof(WetStatus),
        };
    }

    private static void Freeze(Actor actor, bool @bool) {
        
        if (actor is Player player)
            player.InputController.UIInputFilter.SetFilterMode(@bool);
        else if (actor is Enemy enemy)
            enemy.PauseAI = @bool;

    }
    
    public override void Disable(Actor actor) {
        Freeze(actor, false);
        ParticleFactory.RemoveParticle(particle);
    } 
    
    Node2D particle;
    public override void Init(Actor actor) {
        Freeze(actor, true);
        particle = ParticleFactory.AddParticle(actor, Effects.Ice);
    }

    public override void Update(Actor actor, double delta) {}
}