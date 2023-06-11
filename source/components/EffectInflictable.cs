using Godot;
using System;
using System.Collections.Generic;

public partial class EffectInflictable : Node {


    public List<IActorStatus> StatusEffects {get; private set;} = new();
    void ClearAllEffects(DamageInstance _) {
        for (int i = 0; i < StatusEffects.Count; i++) RemoveEffect(StatusEffects[i]);
    }

    private Actor actor;
    
    private void ParseSynergies() {
        //Ice + Fire = Water
    }

    public void Init(Actor actor) {
        this.actor = actor;

        actor.DamageableComponent.OnDamaged += (damageInstance) => Add(damageInstance.statusEffect);
        actor.DamageableComponent.OnDamaged += UpdateEffectHealth;

        actor.DamageableComponent.OnDeath += ClearAllEffects;

        // Debug
        DamageInstance waterDamage = new(actor) {
            damage = 3,
            statusEffect = new WetStatus(),
        };
        DebugHUD.instance.anyButton.Pressed += () => actor.DamageableComponent.Damage(waterDamage);
        // Debug
    }
    private void UpdateEffectHealth(DamageInstance damageInstance) {
        foreach (IActorStatus effect in StatusEffects) {
            effect.OnDamaged(damageInstance);
        }
    }

    private bool EffectIsAllowed(IActorStatus statusEffect) {
        //Add a way to override effects. Should it prioritize strenght? Maybe I must enforce a value for the "strenght"
        //Of the effect?

        //TODO: Temporary solution: If the name is the same, don't add it again.

        foreach (IActorStatus effect in StatusEffects) {
            if (statusEffect.ToString() == effect.ToString()) return false;

            //Idk why this does the thing it does but this is null sometimes?
            if (effect.incompatibles is null) continue;

            foreach (string incompatible in effect.incompatibles) {
                if (statusEffect.ToString() == incompatible) return false;
            }
        }

        return true;
    }
    
    public void Add(IActorStatus effectInstance) {

        // if there's an effect which is a conversion of an effect already here 
        // i.e there's water and we're tryna add fire
        // then we don't add the fire, but instead let the fire damage the water until
        // it replaces it. 

        if (effectInstance is null) return;

        if (!EffectIsAllowed(effectInstance)) return;


        ParseSynergies();
        StatusEffects.Add(effectInstance);
        effectInstance.Init(actor);
        effectInstance.Reset();
    }

    //Called from the actor this is attached to.
    public override void _Process(double delta) {
        
        foreach (IActorStatus effect in StatusEffects.ToArray()) {
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
    private void RemoveEffect(IActorStatus statusEffect) {
        statusEffect.Disable(actor);
        StatusEffects.Remove(statusEffect);
    }
}

public abstract class IActorStatus {


    public abstract float duration {get; protected set;}
    public bool IsPermanent => duration < 0;
    public abstract void Update(Actor actor, double delta);
    public abstract void Init(Actor actor);
    public abstract void Disable(Actor actor);

    public abstract ConvertsTo[] opposites {get; init;}
    public abstract string[] incompatibles {get; init;}

    public double EffectCountdown => duration - effectTime;
    
    private double effectTime;

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

public abstract class IPermanentStatus : IActorStatus {
    // Close Update so that it cannot be edited.
    public override void Update(Actor actor, double delta) {}

    public override float duration {get; protected set;} = -1;
}

public struct ConvertsTo {
    //The type that increases the conversion rate. I.e Fire on Water will cause Gas. Fire is the type.
    public Type type;
    public IActorStatus goTo;
    public ConvertsTo(Type type, IActorStatus goTo) {
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