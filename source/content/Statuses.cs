using Godot;

public sealed class FireEffect : IActorStatus {
    public override float duration {get; protected set;} = 10;
    public override ConvertsTo[] opposites { get; init;}
    //use nameof to get the name of the class that it is incompatible with. 
    public override string[] incompatibles {get; init;}

    public FireEffect() {
        opposites = new ConvertsTo[] {
            new ConvertsTo(typeof(WetStatus), new GasStatus()),
        };

        incompatibles = new string[] {
            nameof(WetStatus),
            nameof(GasStatus)
        };
    }

    private DamageInstance damage;
    
    GpuParticles2D fire;
    public override void Init(Actor actor) {
        
        damage = new(actor) {
            damage = 2,
            suppressImpactFrames = true,
        };

        fire = ParticleFactory.AddParticle(actor, Effects.Fire);
    }
    public override void Disable(Actor actor) =>
        ParticleFactory.RemoveParticle(fire);

    double damageTime;
    public float damagePeriod = 2;

    public override void Update(Actor actor, double delta) {

        damageTime += delta;
        if (damageTime > damagePeriod) {
            damageTime = 0;
            actor.DamageableComponent.Damage(damage);
        }
    }
}

public sealed class WetStatus : IPermanentStatus {

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

    GpuParticles2D water;
    public override void Init(Actor actor) {
        water = ParticleFactory.AddParticle(actor, Effects.Wet);
        actor.MoveSpeed /= 10;
    }

    public override void Disable(Actor actor) {
        actor.MoveSpeed *= 10;
        ParticleFactory.RemoveParticle(water);
    }
}

public sealed class GasStatus : IPermanentStatus {
    public override ConvertsTo[] opposites {get; init;}
    public override string[] incompatibles {get; init;}

    public GasStatus() {
        opposites = new ConvertsTo[] {
            
        };
    }
    public override void Disable(Actor actor) {
        ParticleFactory.RemoveParticle(gas);
    }

    GpuParticles2D gas;
    //TODO: Just make this have a cute visual. Gas state should do nothing paticularly cool. 
    public override void Init(Actor actor) {    
        gas = ParticleFactory.AddParticle(actor, Effects.Gas);
    }
}


// Fire and electricity ? 
// Or fire and gas ?
public sealed class PlasmaEffect : IActorStatus {
    public override float duration {get; protected set;} = 10;
    public override ConvertsTo[] opposites { get; init;}
    //use nameof to get the name of the class that it is incompatible with. 
    public override string[] incompatibles {get; init;}

    public override void Init(Actor actor) { 
        //init the effect too. 
        actor.DamageableComponent.DamageTakenMulitplier += 0.5f;
    }
    public override void Disable(Actor actor) {
        actor.DamageableComponent.DamageTakenMulitplier -= 0.5f;
    }
    public override void Update(Actor actor, double delta) {}
}