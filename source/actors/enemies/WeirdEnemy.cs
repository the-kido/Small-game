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
    [Export]
    private float attackDelay;
    
    public override void Init(AnimationController animationController, AIStateMachine stateMachine) {
        DefaultAttackState attackState = new(pathfinderComponent, spamedBullet, attackDelay);
        PatrolState patrolState = new(pathfinderComponent, HoverAtSpawnPointDistance);
        
        animationController.AddAnimation(new("shoot", 2), ref attackState.OnShoot);
        animationController.AddAnimation(new("idle", 1), ref patrolState.IsIdle);
        animationController.AddAnimation(new("flying", 1), ref patrolState.IsMoving);

        stateMachine.AddState(attackState, patrolState);
        stateMachine.AddState(patrolState, attackState);

        //Set default state.
        stateMachine.ChangeState(patrolState);
    }
}

public class AnimationController {
    
    AnimationPlayer animationPlayer;
     
    public void AddAnimation(AnimationInfo animation, ref Action setEvent) {
        setEvent += () => SetAnimation(animation);
    }

    public void StopCurrentAnimation(ref Action setEvent) {
        setEvent += () => { 
            currentAnimation = AnimationInfo.none;
            //animationPlayer.Stop();
            //animationPlayer.Play("RESET");
        };

        setEvent += () => GD.Print("reset!");
    }

    public AnimationController(AnimationPlayer animationTree) {
        this.animationPlayer = animationTree;
        animationTree.AnimationFinished += OnAnimationComplete;
    }

    AnimationInfo currentAnimation  = AnimationInfo.none;    
    private void SetAnimation(AnimationInfo animation) {
        
        //Animations of the same priority should still override the current animation.
        if (currentAnimation.priority > animation.priority) return;
        if (currentAnimation.name == animation.name) return;


        GD.Print("Animation playing: ", animation.name);

        currentAnimation = animation;
        animationPlayer.SpeedScale = animation.speed;

        animationPlayer.Play("RESET");
        animationPlayer.Stop();
        animationPlayer.Play(animation.name);
    }

    private void OnAnimationComplete(StringName name) {
        currentAnimation = AnimationInfo.none;
    }
}


public class AnimationInfo {
    public readonly int priority;
    public readonly string name;
    public float speed = 1;

    public AnimationInfo(string name, int priority) {
        this.name = name;
        this.priority = priority;
    }
    public static AnimationInfo none = new("", -1);
}