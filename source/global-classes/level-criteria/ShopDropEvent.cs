using System;
using Godot;

namespace Game.LevelContent.Criteria;

[GlobalClass]
public partial class ShopDropEvent : LevelCriteria {
    [Export]
    Shop shop;
    [Export]
    int time = 10;


    public override event Action Finished;

    KidoUtils.Timer timer = KidoUtils.Timer.NONE;
    public override void Start() {
        shop.Drop();

        timer = new(time, false, -1);
        timer.TimeOver += Close;
    }
    public override void _Process(double delta) {
        timer.Update(delta);
    }

    private void Close() {
        timer = KidoUtils.Timer.NONE;
        shop.Stop();

        Finished?.Invoke();
    }
}