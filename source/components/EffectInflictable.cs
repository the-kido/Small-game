using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Actors;
using Game.Damage;

namespace Game.ActorStatuses;

public partial class EffectInflictable : Node {

    [Export]
    private AllStatuses defaultStatus = AllStatuses.None;

    private readonly List<ActorStatus> statusEffects = new();
    public void ClearAllEffects() {
        // InvalidOperationException is thrown if this is a foreach... idk why...
        for (int i = 0; i < statusEffects.Count; i++)
            RemoveEffect(statusEffects[i]);
    }
    
    public bool HasEffect(Type effectType) => statusEffects.Any(effect => effect.GetType() == effectType);

    private Actor actor;
    public void Init(Actor actor) {
        this.actor = actor;

        actor.DamageableComponent.OnDamaged += (damageInstance) => Add(damageInstance.statusEffect);
        actor.DamageableComponent.OnDamaged += UpdateEffectHealth;
        actor.DamageableComponent.OnDeath += (_) => ClearAllEffects();

        if (defaultStatus is not AllStatuses.None)
            AddDefaultEffect(defaultStatus);
    }

    private void AddDefaultEffect(AllStatuses status) {
        ActorStatus newEffect = (ActorStatus) Activator.CreateInstance(ActorStatus.All[(int) status]);
        Add(newEffect, true);
    }

    private void UpdateEffectHealth(DamageInstance damageInstance) {
        foreach (ActorStatus effect in statusEffects) {
            effect.OnDamaged(damageInstance);
        }
    }

    private bool EffectIsAllowed(ActorStatus newEffect) {
        //Add a way to override effects. Should it prioritize strenght? Maybe I must enforce a value for the "strenght"
        //Of the effect?

        //TODO: Temporary solution: If the name is the same, don't add it again.

        foreach (ActorStatus otherEffect in statusEffects) {
            if (newEffect.ToString() == otherEffect.ToString() || otherEffect.ConvertsTo is null) 
                return false;

            foreach (ConvertsTo incompatible in otherEffect.ConvertsTo) {
                if (newEffect.ToString() == incompatible.type.ToString()) 
                    return false;
            }
        }

        return true;
    }
    
    public void Add(ActorStatus effectInstance, bool IsPermanent = false) {
        // if there's an effect which is a conversion of an effect already here 
        // i.e there's water and we're tryna add fire
        // then we don't add the fire, but instead let the fire damage the water until
        // it replaces it. 

        if (effectInstance is null || !EffectIsAllowed(effectInstance))
            return;

        InitializeEffect(effectInstance);

        if (IsPermanent)
            effectInstance.MakePermanent();
    }

    private void InitializeEffect(ActorStatus effectInstance) {
        statusEffects.Add(effectInstance);
        effectInstance.AttachActor(actor);

        effectInstance.Init();
        effectInstance.Reset();
    }

    //Called from the actor this is attached to.
    public override void _Process(double delta) {
        
        foreach (ActorStatus effect in statusEffects.ToArray()) {
            effect.UpdateTimer(delta);
            effect.Update(delta);

            if (effect.EffectCountdown < 0 && !effect.IsPermanent)
                RemoveEffect(effect);

            if (effect.ConvertsTo is not null) {
                foreach (ConvertsTo convertsTo in effect.ConvertsTo) {
                    if (convertsTo.Progress >= 10) {
                        RemoveEffect(effect);
                        Add(convertsTo.goTo);
                    }
                }
            }
        }
    }
    private void RemoveEffect(ActorStatus statusEffect) {
        statusEffect.Disable();
        statusEffects.Remove(statusEffect);
    }
}

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