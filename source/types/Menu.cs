using System;

public interface IMenu {
    public abstract void Enable();
    public abstract void Switch();
    public event Action Disable;
}