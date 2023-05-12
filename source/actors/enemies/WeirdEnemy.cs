using System;
using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public enum EnemyStates {
    Patrolling,
    Attacking
}

public sealed partial class WeirdEnemy : Enemy {
    [Export] 
    public Pathfinder pathfinderComponent;

    

    [Export]
    private int HoverAtSpawnPointDistance = 0;
    [Export]
    private PackedScene spamedBullet;



    private Action OnIdle; 
    private Action OnWalking;
    public override void _Ready() {
        base._Ready();
        
        AttackState attackState = new(pathfinderComponent, spamedBullet);
        PatrolState patrolState = new(pathfinderComponent, HoverAtSpawnPointDistance);

        
        AnimationController.AddAnimation("shoot", ref attackState.OnShoot);
        AnimationController.AddAnimation("idle", ref OnIdle);
        AnimationController.AddAnimation("flying", ref OnWalking);
        //animationTree.Set("parameters/conditions/IsShooting", true);

        StateMachine.AddState(attackState, patrolState);
        StateMachine.AddState(patrolState, attackState);

        StateMachine.ChangeState(patrolState);
    }


    public void PlayAnimations() {
        Vector2 playerPosition = Player.players[0].GlobalPosition;
        //These animations should only control:
        //  Idle
        //  Walking
        //  Hurt
        //...Animations and should allow other animations to override the one currently being played.
        //The names should be the same for each, so a check in "ready" can possibly find if the animations are built in yet. 
    
        //If the player shoots, clear everything else.
        //If the player is walking or idling, calculate that only if the shooty is not being run.

        if (animationPlayer.CurrentAnimation != "")

        if (Velocity.IsEqualApprox(Vector2.Zero)) {
            OnIdle?.Invoke();
        } 
        else {
            OnWalking?.Invoke();
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        PlayAnimations();
    }

}

public class AnimationController {
    
    AnimationPlayer animationPlayer;

    public void AddAnimation(string animationName, ref Action setEvent) {
        setEvent += () => SetAnimation(animationName);
    }

    public AnimationController(AnimationPlayer animationTree) {
        this.animationPlayer = animationTree;
    }


    private void SetAnimation(string animationName) {
        

        if (animationPlayer.CurrentAnimation != "") return;


        animationPlayer.Stop();

        //Add the animation to the queue.
        //If the queue has another item, remove it so that this new one plays instead.
        animationPlayer.ClearQueue();
        animationPlayer.Queue(animationName);
    }
}
