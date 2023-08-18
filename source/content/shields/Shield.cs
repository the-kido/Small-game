using Godot;
using System;
using Game.Damage;

namespace Game.Players.Mechanics;

public abstract partial class Shield : Node2D, IChestItem {
    public Texture2D Icon {get => sprite.Texture;}
    public abstract string Description {get;}
    
    // TOOD: This name is prolly too vague
    public event Action Updated;

    public ChestItemType Type => ChestItemType.SHIELD;

    [Export]
    private Sprite2D sprite;
    [Export]
    int maxHealth;
    public int Health {get; private set;}

    public bool Alive => Health > 0;

    [Export]
    float healSpeed;

    public void Heal(int healthAdded) {
        Health = (int) MathF.Min(Health + healthAdded, maxHealth);
        Updated?.Invoke();
    } 

    public void Init() {
        Health = maxHealth;
    }

    public Shield() {
        timer.TimeOver += () => Heal(1);
    }

    KidoUtils.Timer timer = new(1);
    public virtual void Update(double delta) {
        timer.Update(delta);
    }

    public virtual void Use(DamageInstance damageInstance) {
        Health = (int) MathF.Max(Health - damageInstance.damage, 0);
        timer.Pause(2);

        if (!Alive) timer.Pause(5);

        Updated?.Invoke();
    }
}
