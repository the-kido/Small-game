using Godot;
using System;

public partial class Gun : Weapon {
    [Export]
    public PackedScene bulletAsset; 
    private Node2D nuzzle;
    public override void _Ready() {
        base._Ready();
        //bulletAsset = (PackedScene) GD.Load("res://source/weapons/bullets/base_bullet.tscn");
        nuzzle = (Node2D) GetNode("Nuzzle");
    }
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        FaceWeaponToCursor();
        
    }
	public virtual void FaceWeaponToCursor() {
		hand.LookAt(GetGlobalMousePosition());
	}
    
    public override void useWeapon(string[] inputMap) {

        var newBullet = GetNode<BulletFactory>("/root/BulletFactory").SpawnBullet(bulletAsset);
         //= BulletFactory.SpawnBullet(bulletAsset);
    	newBullet.init(nuzzle.GlobalPosition, nuzzle.GlobalRotation);

    }
}
