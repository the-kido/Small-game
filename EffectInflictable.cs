using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public enum Element {
    Hot,
    Cold,
    Warm,

}

public partial class EffectInflictable : Node {


    #region All exported objects (shaders and the like) 
    
    #endregion

    #region  relavent fields
    private List<IStatusEffect> statusEffects = new();
    private float tempurature;
    private Actor actor;
    private Damageable damageable;
    #endregion

    public void Init(Actor actor, Damageable damageable) {
        this.actor = actor;
        this.damageable = damageable;
    }

    public void Add(IStatusEffect effectInstance) {
        
        //Add a way to override effects. Should it prioritize strenght? Maybe I must enforce a value for the "strenght"
        //Of the effect?
        foreach (var effect in statusEffects) {
            //Temporary solution: If the name is the same, don't add it again.
            if (effect.ToString() == effectInstance.ToString()) return;
        }

        statusEffects.Add(effectInstance);
        effectInstance.Init(actor);

        StopStatusEffect(effectInstance);
    }

    private async void StopStatusEffect(IStatusEffect effect) {
		await Task.Delay( (int) (effect.duration) * 1000);
		statusEffects.Remove(effect);
        effect.Finish(actor);
    }
}


public interface IStatusEffect {
    public float duration {get; set;}
    public void Init(Actor actor);
    public void Finish(Actor actor);
}

public record PoisonEffect : IStatusEffect {

    public float duration {get; set;} = 10;
    public int damageTaken = 2;
    public float time = 2;
    //The GC should clean this up when it's left the list.
    
    
    public async void RecurseDamage(Actor actor) {
        DamageInstance damage = new DamageInstance {
            damage = damageTaken,
        };

        await Task.Delay((int) time * 1000);

        if (stopRecurse) return;

        actor.DamageableComponent.Damage(damage);
        RecurseDamage(actor);
    }

    public void Init(Actor actor) {
        RecurseDamage(actor);
    }
    bool stopRecurse = false;

    public void Finish(Actor actor) {
        stopRecurse = true;
    }
}

public record NoEffect : IStatusEffect
{
    public float duration {get; set;} = 0;
    public void Finish(Actor actor) {
    }

    public void Init(Actor actor) {
    }
}