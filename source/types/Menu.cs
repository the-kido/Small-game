using System;

public interface IMenu {
    public abstract void Enable(Player player);
    public abstract void Switch();
    public event Action Disable;
}