using System;
using Game.Players;
using Godot;
using System.Linq;

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
        
        // Show the first menu
        currentIndex = 0;
        SetToNewClass(0);

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

    int currentIndex = 0;

    private void SetToNewClass(int index) {
        var temp = PlayerClasses.List.Values.ToList();
        classInfo.Text = temp[index].Discription;
        
        var animations = (Godot.Collections.Dictionary) temp[index].playerSprites.Animations[0];
        var frames = (Godot.Collections.Array) animations["frames"];
        var firstFrame = (Godot.Collections.Dictionary) frames[0];

        textureRect.Texture = (AtlasTexture) firstFrame["texture"]; 
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