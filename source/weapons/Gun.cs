using Godot;
using System.Collections.Generic;
using KidoUtils;
using System.Threading.Tasks;

public partial class Gun : Weapon {
    [Export]
    public PackedScene bulletAsset; 
    private Node2D nuzzle;
    public override void _Ready() {
        base._Ready();
        nuzzle = (Node2D) GetNode("Nuzzle");
    }
    
    public override void _Process(double delta)
    {
        base._Process(delta);
    }
    
	public virtual void FaceWeaponToCursor() {
        hand.LookAt(GetGlobalMousePosition());
	}

    uint mask = (uint) Layers.Enviornment + (uint) Layers.Enemies;


    private bool waiting = false;
    private Actor FindObjectToFace(List<Actor> enemies) {
        //Check if the player is clicking/pressing on the screen. 
        foreach (Actor enemy in Player.players[0].NearbyEnemies) {

            PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
            var ray = PhysicsRayQueryParameters2D.Create(GlobalPosition, enemy.GlobalPosition, mask);
            var result = spaceState.IntersectRay(ray);

            if (result.Count > 0 && (Rid) result["collider"] == enemy.GetRid())
                return enemy;
        }
        return null;
    }

    public override void UpdateWeapon(List<InputType> inputMap) {
        if (inputMap.Contains(InputType.AttackButtonPressed)) {
            Actor see = FindObjectToFace(Player.players[0].NearbyEnemies);
            
            if (see is not null) {
                hand.LookAt(see.GlobalPosition);
                AttackAndReload(false);
            }
            return;
        }

        if (inputMap.Contains(InputType.LeftClick)) {
            FaceWeaponToCursor();
            AttackAndReload(true);
        }
    }

    public override void UseWeapon() {
        
        GetNode<BulletFactory>("/root/BulletFactory").SpawnBullet(bulletAsset)
            .init(nuzzle.GlobalPosition, nuzzle.GlobalRotation, BulletFrom.Player);
        Camera.currentCamera.StartShake((float) DebugHUD.instance.anySlider.Value, 300, 1);
    }
}
