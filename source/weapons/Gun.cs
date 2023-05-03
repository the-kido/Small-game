using Godot;
using System.Collections.Generic;
using KidoUtils;

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
        //Check if the player is clicking/pressing on the screen. 

        Actor see = null;
        foreach (Actor enemy in Player.players[0].NearbyEnemies) {
            PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
            var ray = PhysicsRayQueryParameters2D.Create(nuzzle.GlobalPosition, enemy.GlobalPosition, mask);
            var result = spaceState.IntersectRay(ray);


            if (result.Count > 0 && (Rid) result["collider"] == enemy.GetRid())
                see = enemy;
                break;
        }
        GD.Print(see);
        if (see is not null)
            hand.LookAt(see.GlobalPosition);
        else
		    hand.LookAt(GetGlobalMousePosition());

	}
    uint mask = (uint) Layers.Enviornment + (uint) Layers.Enemies;
    public void FaceWeaponToNearbyEnemy(List<Actor> enemies) {
        
    }
    


    public override void useWeapon(string[] inputMap) {

        var newBullet = GetNode<BulletFactory>("/root/BulletFactory").SpawnBullet(bulletAsset);
         //= BulletFactory.SpawnBullet(bulletAsset);
    	newBullet.init(nuzzle.GlobalPosition, nuzzle.GlobalRotation, BulletFrom.Player);

    }
}
