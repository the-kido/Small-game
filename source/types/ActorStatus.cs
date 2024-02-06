using System;
using Game.Actors;
using Game.Damage;

namespace Game.ActorStatuses;

public abstract partial class ActorStatus {

    protected Actor actorAttachedTo;

    public abstract float Duration {get; protected set;}
    public bool IsPermanent => Duration < 0;

    public void AttachActor(Actor actor) => actorAttachedTo = actor;
    
    public virtual void Update(double delta) {}
    public abstract void Init();
    public abstract void Disable();
    public virtual int GetEffectSynergyDamageBonus() => 0;

    public abstract ConvertsTo[] ConvertsTo {get; init;}
    public abstract Type[] Incompatibles {get; init;}

    public double EffectCountdown => Duration - effectTime;
    
    private double effectTime;

    public void MakePermanent() {
        Duration = -1;
    }

    public void Reset() {
        effectTime = 0;
    }

    public void UpdateTimer(double delta) {
        effectTime += delta;
    }
    
    public void OnDamaged(DamageInstance damage) {
        if (ConvertsTo is null) 
            return;
        
        for (int i = 0; i < ConvertsTo.Length; i++) {
            if (damage.statusEffect is null) 
                continue;
            
            if (damage.statusEffect.GetType() == ConvertsTo[i].type) {
                ConvertsTo[i].IncreaseProgress(damage.damage);
                return;
            }
        } 
    }
}

public abstract class PermanentStatus : ActorStatus {
    // Close Update so that it cannot be edited.
    public override void Update(double delta) {}

    public override float Duration {get; protected set;} = -1;
}

public struct ConvertsTo {
    //The type that increases the conversion rate. I.e Fire on Water will cause Gas. Fire is the type.
    public Type type;
    public ActorStatus goTo;
    public ConvertsTo(Type type, ActorStatus goTo) {
        this.type = type;
        this.goTo = goTo;
        Progress = 0;
    }
    //From 0 to 10. 10 Damage units
    public float Progress {get; private set;}
    public void IncreaseProgress (int value) {
        Progress += value;
    }
}