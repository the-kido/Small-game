using Godot;

public abstract partial class Shield : Node2D, IChestItem {
    public Texture2D Icon {get => sprite.Texture;}
    public abstract string Description {get;}

    public ChestItemType Type => ChestItemType.SHIELD;

    [Export]
    private Sprite2D sprite;
    [Export]
    int maxHealth;
    public int Health {get; private set;}

    public bool Alive => Health > 0;

    [Export]
    float healSpeed;

    public void Init() {
        GD.Print("does this ever get called");
        Health = maxHealth; 
    }

    public virtual void Use(DamageInstance damageInstance) {
        Health -= damageInstance.damage;
    }
}
