using System;
using Game.Players;

namespace Game.UI;

public interface IMenu {
    public abstract void Enable(Player player);
    public abstract void Switch();
    public event Action Disable;
}