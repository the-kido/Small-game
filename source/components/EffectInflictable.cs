using Godot;
using System;
using System.Collections.Generic;
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

    private bool EffectIsAllowed(ActorStatus statusEffect) {
        //Add a way to override effects. Should it prioritize strenght? Maybe I must enforce a value for the "strenght"
        //Of the effect?

        //TODO: Temporary solution: If the name is the same, don't add it again.

        foreach (ActorStatus effect in statusEffects) {
            if (statusEffect.ToString() == effect.ToString()) return false;

            //Idk why this does the thing it does but this is null sometimes?
            if (effect.incompatibles is null) continue;

            foreach (string incompatible in effect.incompatibles) {
                if (statusEffect.ToString() == incompatible) return false;
            }
        }

        return true;
    }
    
    public void Add(ActorStatus effectInstance, bool IsPermanent = false) {
        
        // if there's an effect which is a conversion of an effect already here 
        // i.e there's water and we're tryna add fire
        // then we don't add the fire, but instead let the fire damage the water until
        // it replaces it. 

        if (effectInstance is null) return;

        if (!EffectIsAllowed(effectInstance)) return;

        statusEffects.Add(effectInstance);
        effectInstance.Init(actor);
        effectInstance.Reset();

        if (IsPermanent)
            effectInstance.MakePermanent();
    }

    //Called from the actor this is attached to.
    public override void _Process(double delta) {
        
        foreach (ActorStatus effect in statusEffects.ToArray()) {
            effect.UpdateTimer(delta);
            effect.Update(actor, delta);

            foreach (ConvertsTo convertsTo in effect.opposites) {
                if (convertsTo.Progress >= 10) {
                    RemoveEffect(effect);
                    Add(convertsTo.goTo);
                }
            }

            if (effect.EffectCountdown < 0 && !effect.IsPermanent)
                RemoveEffect(effect);
        }
    }
    private void RemoveEffect(ActorStatus statusEffect) {
        statusEffect.Disable(actor);
        statusEffects.Remove(statusEffect);
    }
}

public abstract partial class ActorStatus {

    public abstract float Duration {get; protected set;}
    public bool IsPermanent => Duration < 0;
    public abstract void Update(Actor actor, double delta);
    public abstract void Init(Actor actor);
    public abstract void Disable(Actor actor);

    public abstract ConvertsTo[] opposites {get; init;}
    public abstract string[] incompatibles {get; init;}

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
        for (int i = 0; i < opposites.Length; i++) {
            if (damage.statusEffect is null) continue;
            
            if (damage.statusEffect.GetType() == opposites[i].type) {
                opposites[i].IncreaseProgress(damage.damage);
                return;
            }
        } 
    }
}

public abstract class PermanentStatus : ActorStatus {
    // Close Update so that it cannot be edited.
    public override void Update(Actor actor, double delta) {}

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