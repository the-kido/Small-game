using Godot;
using System;

namespace Game.Animation;

public class AnimationController {
     
    public void AddAnimation(AnimationInfo animation, ref Action setEvent) {
        setEvent += () => SetAnimation(animation);
    }

    //I literally don't know how to solve this issue so this is what you're getting.
    bool animationPlayerFreed = false;

    readonly AnimationPlayer animationPlayer;
    public AnimationController(AnimationPlayer animationPlayer) {
        this.animationPlayer = animationPlayer;
        this.animationPlayer.TreeExited += () => animationPlayerFreed = true;

        animationPlayer.AnimationFinished += OnAnimationComplete;
    }

    AnimationInfo currentAnimation = AnimationInfo.none;

    private bool CanSetAnimation(AnimationInfo animation) {
        if (animationPlayerFreed) return false;

        //Animations of the same priority should still override the current animation.
        if (currentAnimation.priority > animation.priority) return false;
        if (currentAnimation.name == animation.name) return false;

        return true; // Allows animation to be set after all checks pass
    }

    private void SetAnimation(AnimationInfo animation) {
        if (!CanSetAnimation(animation)) return;

        currentAnimation = animation;
        
        animationPlayer.SpeedScale = animation.speed;

        if (animation.resetPreviousAnimation) {
            animationPlayer.Play("RESET");
            animationPlayer.Stop();
        }

        animationPlayer.Play(animation.name);
    }

    private void OnAnimationComplete(StringName name) => currentAnimation = AnimationInfo.none;
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

    public static readonly AnimationInfo none = new("", -1);
}
