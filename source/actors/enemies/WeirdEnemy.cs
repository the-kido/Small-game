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

    public override void _Ready() {
        base._Ready();
        
        DefaultAttackState attackState = new(pathfinderComponent, spamedBullet, attackDelay);
        PatrolState patrolState = new(pathfinderComponent, HoverAtSpawnPointDistance);

        
        AnimationController.AddAnimation(new("shoot", 2), ref attackState.OnShoot);
        AnimationController.AddAnimation(new("idle", 1), ref patrolState.IsIdle);
        AnimationController.AddAnimation(new("flying", 1), ref patrolState.IsMoving);

        patrolState.IsMoving += () => GD.Print("is moving");
        patrolState.IsIdle += () => GD.Print("is idle");

        //animationTree.Set("parameters/conditions/IsShooting", true);

        StateMachine.AddState(attackState, patrolState);
        StateMachine.AddState(patrolState, attackState);

        StateMachine.ChangeState(patrolState);
    }

}

public class AnimationController {
    
    AnimationPlayer animationPlayer;

    public void AddAnimation(AnimationInfo animation, ref Action setEvent) {
        setEvent += () => SetAnimation(animation);
    }

    public AnimationController(AnimationPlayer animationTree) {
        this.animationPlayer = animationTree;
        animationTree.AnimationFinished += OnAnimationComplete;
    }

    AnimationInfo currentAnimation;    
    private void SetAnimation(AnimationInfo animation) {
        
        //Animations of the same priority should still override the current animation.
        if (currentAnimation.priority > animation.priority) return;
        if (currentAnimation.name == animation.name) return;

        currentAnimation = animation;
        animationPlayer.Play("RESET");
        animationPlayer.Stop();
        animationPlayer.Play(animation.name);
    }

    private void OnAnimationComplete(StringName name) {
        currentAnimation = AnimationInfo.none;
    }
}

public struct AnimationInfo {
    public int priority;
    public string name;

    public AnimationInfo(string name, int priority) {
        this.name = name;
        this.priority = priority;
    }

    public static AnimationInfo none = new("", -1);
}