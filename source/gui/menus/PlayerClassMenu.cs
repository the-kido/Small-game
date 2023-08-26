using System;
using Game.Players;
using Godot;
using System.Linq;
using Game.Actors;

namespace Game.UI;

public sealed partial class PlayerClassMenu : ColorRect, IMenu {
    [Export]
    private Button close;

    [Export]
    private Button left;
    [Export]
    private Button right;

    [Export]
    private RichTextLabel classInfo;

    [Export]
    private TextureRect textureRect;

    [Export]
    private Button changeClass;

    public event Action Disable;

    private Player player;

    public void Enable(Player player) {
        this.player = player;
        Visible = true;

        left.Pressed += () => {
            currentIndex = (currentIndex - 1 + PlayerClasses.List.Count) % PlayerClasses.List.Count;
            SetToNewClass(currentIndex);
        };

        right.Pressed += () => {
            currentIndex = (currentIndex + 1) % PlayerClasses.List.Count;
            SetToNewClass(currentIndex);
        };

        close.Pressed += OnDisable;

        changeClass.Pressed += SwitchClass;
    }
    int currentIndex;

    private void SetToNewClass(int index) {
        var temp = PlayerClasses.List.Values.ToList();
        classInfo.Text = temp[index].Discription;
    }

    private void SwitchClass() {
        PlayerManager.SwitchClass(PlayerClasses.List.Keys.ToArray()[currentIndex], player.GlobalPosition);
        OnDisable();
    }

    private void OnDisable() {
        close.Pressed -= OnDisable;
        Disable?.Invoke();
    }

    public void Close() {
        Visible = false;
    }
} 