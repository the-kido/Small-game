using System;
using Game.Players;

namespace Game.UI;

public interface IMenu {
    public abstract void Enable(Player player);
    public abstract void Close();

    public Action Disable {get; set;}
}