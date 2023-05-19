using Godot;
using System;
using System.Threading.Tasks;

public partial class AgroEnemyMiniBoss : Enemy
{
    [Export]
    private Pathfinder pathfinder;

    [Export]
    private Area2D rushCollisionArea;
    
    public override void Init(AnimationController animationController, AIStateMachine stateMachine) {
        PatrolState patrolState = new(pathfinder, 400);
        AnimationInfo runningAnimation = new("Running", 1) {speed = 4};

        AgroEnemyRushState rushState = new(rushCollisionArea, runningAnimation);

        stateMachine.AddState(rushState, patrolState);
        stateMachine.AddState(patrolState, rushState);

        stateMachine.ChangeState(patrolState);

        animationController.StopCurrentAnimation(ref rushState.OnStateChanged);

        animationController.StopCurrentAnimation(ref patrolState.IsIdle);

        patrolState.IsIdle += () => GD.Print("Is idle");
        patrolState.IsMoving += () => GD.Print("Is moving");

        animationController.AddAnimation(new("Running", 1), ref patrolState.IsMoving);

        animationController.AddAnimation(new("Preping", 1), ref rushState.OnPreparingToRush);
        animationController.AddAnimation(runningAnimation, ref rushState.OnRushing);
        animationController.AddAnimation(new("fatigue begin", 1), ref rushState.OnFallsTired);
        animationController.AddAnimation(new("Wake up", 2), ref rushState.OnWakesUp);
    }
}

public sealed class AgroEnemyRushState : AIState {

    private Area2D rushCollisionArea {get; init;}
    private AnimationInfo runningAnimation {get; init;}
    public AgroEnemyRushState (Area2D rushCollisionArea, AnimationInfo runningAnimation) {
        this.rushCollisionArea = rushCollisionArea;
        this.runningAnimation = runningAnimation;
    }

    enum State {
        Preping,
        Rushing,
        Fatigued,
        //When the AI finall stops rushing the player and stops being tired. It wakes up.
        Done,
    }
    State state = State.Done; 

    DamageInstance damage = new() {
        damage = 10,
    };

    private void OnBodyEntered(Node2D body) {
        if (body is Player player) {
            damage.forceDirection = actor.Velocity;
            player.DamageableComponent.Damage(damage);
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
    int slowDownBeginTime = 8; //seconds
    double slowDownLength = 4; //seconds
    private void Rush() {
        //The 3 acts as a buffer just so that the monster doesnt start to slow down immediately.
        double mathxd = (slowDownLength - (time - slowDownBeginTime)) / slowDownLength;
        double multiplier = Math.Min(mathxd, 1) * 10;

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
    private Player targettedPlayer;

    private void OnStateSwitched(State state) {
        actor.Velocity = Vector2.Zero;
        switch (state) {
            case State.Preping:
                OnPreparingToRush?.Invoke();
                break;

            case State.Rushing:
                //Set the initial rush direction
                rushDirection = (Player.players[0].GlobalPosition - actor.GlobalPosition).Normalized();
                
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
        
        //rushCollisionArea.BodyEntered += OnBodyEntered;
        
        //Might use this? Or, will try to use NORMALS to make a more accurate direction for the bouncy boi to go.
        rushCollisionArea.GetOverlappingBodies();
        
        time = 0;
    }
    private async void LeaveState() {
        OnWakesUp?.Invoke();

        //Wait for the wake up animation to play first.
        await Task.Delay(1000);
        
        stateMachine.ChangeState(stateToGoTo);
        
        //rushCollisionArea.BodyEntered -= OnBodyEntered;
    }

    private void FlipActor(Vector2 direction) {
        bool flip = MathF.Sign(direction.X) == 1 ? false : true;
        actor.Flip(flip);
    }
}