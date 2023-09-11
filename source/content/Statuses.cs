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
    None,
    FireEffect,
    WetStatus,
    ShieldedStatus,
    GasStatus,
    PlasmaEffect,
    FreezeEffect,
}

// Just for the list
public abstract partial class ActorStatus {
    public static readonly List<Type> All = new() {
        null, typeof(FireEffect), typeof(WetStatus), typeof(ShieldedStatus), typeof(GasStatus), typeof(PlasmaEffect), typeof(FreezeEffect) 
    };
}

public sealed class FireEffect : ActorStatus {
    public override float Duration {get; protected set;} = 10;
    
    public override ConvertsTo[] ConvertsTo { get; init;} = new ConvertsTo[] {
        new(typeof(WetStatus), new GasStatus()),
    };

    public override Type[] Incompatibles {get; init;} = new Type[] {
        typeof(WetStatus),
        typeof(GasStatus)
    };

    private DamageInstance damage;
    
    Node2D fire;
    public override void Init() {
        
        damage = new(actorAttachedTo) {
            damage = 2,
            suppressImpactFrames = true,
        };
        
        // actor.Modulate += new Color(0.3f, 0.2f, 0f);
        fire = ParticleFactory.AddParticle(actorAttachedTo, Effects.Fire);
    }
    public override void Disable() {
        // actor.Modulate -= new Color(0.3f, 0.2f, 0f);

        ParticleFactory.RemoveParticle(fire);
    }

    double damageTime;
    public float damagePeriod = 2;

    public override void Update(double delta) {
        
        damageTime += delta;
        if (damageTime > damagePeriod) {
            damageTime = 0;

            if (actorAttachedTo.DamageableComponent?.ImmuneToDamageFrom?.Contains(AllStatuses.FireEffect) ?? false) 
                return;

            actorAttachedTo.DamageableComponent.Damage(damage);
        }
    }
}

public sealed class WetStatus : PermanentStatus {

    public override ConvertsTo[] ConvertsTo {get; init;} = new ConvertsTo[] {
        new(typeof(FireEffect), new GasStatus()),
    };

    public override Type[] Incompatibles {get; init;} = new Type[] {
        typeof(FireEffect),
    };

    Node2D water;
    
    ActorStats debuff = new() {
        speed = new(0.1f, 0), // 10 times slower.
        damageDealt = new(1f, -0.2f) 
    }; 
    
    public override void Init() {
        water = ParticleFactory.AddParticle(actorAttachedTo, Effects.Wet);
        actorAttachedTo.StatsManager.AddStats(debuff);
    }

    public override void Disable() {
        actorAttachedTo.StatsManager.RemoveStats(debuff);
        ParticleFactory.RemoveParticle(water);
    }
}

public sealed class ShieldedStatus : PermanentStatus {
    public override ConvertsTo[] ConvertsTo {get; init;} = Array.Empty<ConvertsTo>();
    public override Type[] Incompatibles {get; init;} = Array.Empty<Type>();

    ActorStats buff = new() {
        damageTaken = new(0f, -100),
    };

    public override void Disable() {
        ParticleFactory.RemoveParticle(particle);
        actorAttachedTo.StatsManager.RemoveStats(buff);
    }

    Node2D particle;

    public override void Init() {
        actorAttachedTo.StatsManager.AddStats(buff);
        particle = ParticleFactory.AddParticle(actorAttachedTo, Effects.Shield);
    }
}

public sealed class GasStatus : PermanentStatus {
    public override ConvertsTo[] ConvertsTo {get; init;} = new ConvertsTo[] {
        new(typeof(FireEffect), new PlasmaEffect()),  
    };

    public override Type[] Incompatibles {get; init;} 

    public override void Disable() {
        ParticleFactory.RemoveParticle(gas);
    }

    private Node2D gas;
    //TODO: Just make this have a cute visual. Gas state should do nothing paticularly cool. 
    public override void Init() {    
        gas = ParticleFactory.AddParticle(actorAttachedTo, Effects.Gas);
    }
}

// Fire and electricity ? 
// Or fire and gas ?
public sealed class PlasmaEffect : ActorStatus {
    public override float Duration {get; protected set;} = 10;
    public override ConvertsTo[] ConvertsTo { get; init;}
    //use nameof to get the name of the class that it is incompatible with. 
    public override Type[] Incompatibles {get; init;}

    ActorStats buff = new() {
        damageTaken = new(0, 0.5f),
    };

    public override void Init() { 
        //init the effect too. 
        actorAttachedTo.StatsManager.AddStats(buff);
    }
    public override void Disable() {
        actorAttachedTo.StatsManager.RemoveStats(buff);
    }
    public override void Update(double delta) {}
}

public sealed class PoisonStatus : ActorStatus {
    public override float Duration {get; protected set; } = 5;
    public override ConvertsTo[] ConvertsTo {get; init;}
    public override Type[] Incompatibles {get; init;}

    public override void Disable() => timer.TimeOver -= () => Damage(actorAttachedTo);
    public override void Init() => timer.TimeOver += () => Damage(actorAttachedTo);

    private static DamageInstance GetDamageInstance(Actor actor) => new(actor) {
        damage = 1,
    };

    private static void Damage(Actor actor) => actor.DamageableComponent.Damage(GetDamageInstance(actor));

    KidoUtils.Timer timer = new(5);
    public override void Update(double delta) => timer.Update(delta);
    
    // Take extra damage from water damage
    public override int GetEffectSynergyDamageBonus() => actorAttachedTo.Effect.HasEffect(typeof(WetStatus)) ? 1 : 0;
}

public sealed class FreezeEffect : ActorStatus {
    public override float Duration {get; protected set; } = 5;
    
    public override ConvertsTo[] ConvertsTo {get; init;} = new ConvertsTo[] {
        new(typeof(FireEffect), new WetStatus()),
    };

    public override Type[] Incompatibles {get; init;} = new Type[] {
        typeof(FireEffect),
        typeof(WetStatus),
    };

    private static void Freeze(Actor actor, bool @bool) {
        
        if (actor is Player player)
            player.InputController.UIInputFilter.SetFilterMode(@bool);
        else if (actor is Enemy enemy)
            enemy.PauseAI = @bool;
    }
    
    public override void Disable() {
        Freeze(actorAttachedTo, false);
        ParticleFactory.RemoveParticle(particle);
    } 
    
    Node2D particle;
    public override void Init() {
        Freeze(actorAttachedTo, true);
        particle = ParticleFactory.AddParticle(actorAttachedTo, Effects.Ice);
    }

    public override void Update(double delta) {}
}