using Godot;
using System;
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

        currentAnimation = animation;
        animationPlayer.SpeedScale = animation.speed;

        if (animation.resetPreviousAnimation) {
            animationPlayer.Play("RESET");
            animationPlayer.Stop();
        }
        
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

    public bool resetPreviousAnimation = true;
    

    public AnimationInfo(string name, int priority) {
        this.name = name;
        this.priority = priority;
    }
    public static AnimationInfo none = new("", -1);
}