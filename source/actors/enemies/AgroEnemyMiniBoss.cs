using Godot;
using LootTables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class AgroEnemyMiniBoss : Enemy
{
    [Export]
    private Pathfinder pathfinder;

	protected override List<Loot> DeathDrops {get; init;} = LootTable.GENERIC_ENEMY_DROPS;


	protected sealed override void Init(AnimationController animationController, AIStateMachine stateMachine) {
        AnimationInfo runningAnimation = new("Running", 1) {speed = 4};
        
        PatrolState patrolState = new(pathfinder, 400);
        AgroEnemyRushState rushState = new();

        stateMachine.AddState(rushState, patrolState);
        stateMachine.AddState(patrolState, rushState);

        stateMachine.ChangeState(patrolState);

        animationController.AddAnimation(new("RESET", 1), ref patrolState.IsIdle);
        animationController.AddAnimation(new("Running", 1), ref patrolState.IsMoving);
        
        // This isnt even needed? 
        // animationController.StopCurrentAnimation(ref rushState.OnStateChanged);
        
        animationController.AddAnimation(new("Preping", 1), ref rushState.OnPreparingToRush);
        animationController.AddAnimation(runningAnimation, ref rushState.OnRushing);
        animationController.AddAnimation(new("fatigue begin", 1), ref rushState.OnFallsTired);
        animationController.AddAnimation(new("Wake up", 2) {resetPreviousAnimation = false}, ref rushState.OnWakesUp);
    }

    public override void _Ready() {
        base._Ready();

        // DialogueLine[] dialogue = new DialogueLine[] {new("HELLO?", Portraits.boss["Happy"]), new(".... :( ", Portraits.boss["Sad"])};
        DialogueLine[] dialogue = DialogueLines.Lines["Kill Agro Boss"];
        // TODO: OK this is a problem.
        DamageableComponent.OnDeath += (_) => Player.Players[0].GUI.DialoguePlayer.Start(dialogue, new());
        DamageableComponent.OnDeath += (_) => ResourceLoader.Load<Condition>("res://assets/levels/debug/weird_door.tres").OnConditionAchieved?.Invoke();
        
    }
}

public sealed class AgroEnemyRushState : AIState {

    
    State state = State.Done; 

    DamageInstance Damage => new(actor) {
        damage = 10,
    };

    private void OnBodyEntered(Node2D body) {
        if (body is Player player) {
            Damage.forceDirection = actor.Velocity;
            player.DamageableComponent.Damage(Damage);
        }
    }
   
    private void Bounce(Vector2 normal) {
        
        //Y goes down on godot, rememeber lol.

        Vector2 newDirection = normal;

        float rand = (new Random().NextSingle() - 0.5f) * 1.9f;

        if (newDirection.X is 0) {
            newDirection.X = rand;
        }
        else if (newDirection.Y is 0) {
            newDirection.Y = rand;
        }

        rushDirection = newDirection.Normalized();
    }
    Vector2 rushDirection;
    
    //Make sure you update this to reflex the lenght of the rush mode.
    const int SLOW_DOWN_BEGIN_TIME = 8; //seconds
    const double SLOW_DOWN_LENGTH = 4; //seconds
    private void Rush() {
        //The 3 acts as a buffer just so that the monster doesnt start to slow down immediately.
        double mathxd = (SLOW_DOWN_LENGTH - (time - SLOW_DOWN_BEGIN_TIME)) / SLOW_DOWN_LENGTH;
        double multiplier = Math.Min(mathxd, 1) * actor.MoveSpeed/10;

        Vector2 velocity = (rushDirection) * (float) multiplier; 
        
        KinematicCollision2D a = actor.MoveAndCollide(velocity);
        FlipActor(rushDirection);

        if (a is not null) {
            OnBodyEntered((Node2D) a.GetCollider());
            Bounce(a.GetNormal());
        }
    }

    double time = 0;
    //              Loop                Loop        Once        Loop        Once
    public Action OnPreparingToRush, OnRushing, OnFallsTired, WhileTired, OnWakesUp;

    private void UpdateState() {
        State oldState = state;

        //THIS IS LTIERALLY RUST WAHT THE ?! COOL
        state = time switch {
            //0 -- 2 -- 12 -- 15
            //1 4 and 6 are debugging values
            <= 2           => State.Preping,
            > 2 and < 12    => State.Rushing,
            > 12 and < 16     => State.Fatigued,
            _              => State.Done
        };

        if (oldState != state) {
            OnStateChanged?.Invoke();
            OnStateSwitched(state);
        } 
    }
    public Action OnStateChanged;

    private void OnStateSwitched(State state) {
        actor.Velocity = Vector2.Zero;
        switch (state) {
            case State.Preping:
                OnPreparingToRush?.Invoke();
                break;

            case State.Rushing:
                //Set the initial rush direction
                rushDirection = (Player.Players[0].GlobalPosition - actor.GlobalPosition).Normalized();
                
                OnRushing?.Invoke();
                break;

            case State.Fatigued:
                OnFallsTired?.Invoke();
                break;

            case State.Done:
                LeaveState();
                break;
        }
    }

    public override void Update(double delta) {
        time += delta;
        UpdateState();

        if (state is State.Fatigued) {
            WhileTired?.Invoke();
        }

        if (state is State.Rushing) {
            Rush();
        }
    }
    
    public override void Init() {
        actor.Velocity = Vector2.Zero;
        time = 0;
    }

    private async void LeaveState() {
        OnWakesUp?.Invoke();

        //Wait for the wake up animation to play first.
        await Task.Delay(1000);
        
        stateMachine.ChangeState(stateToGoTo);
    }

    private void FlipActor(Vector2 direction) {
        bool flip = MathF.Sign(direction.X) != 1;
        actor.Flip(flip);
    }

    enum State {
        Preping,
        Rushing,
        Fatigued,
        //When the AI finall stops rushing the player and stops being tired. It wakes up.
        Done,
    }
}