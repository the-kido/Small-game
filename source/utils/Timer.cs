using System;

namespace KidoUtils;

public struct Timer {

    public static readonly Timer NONE = new() { 
        timerPause = new() {paused = true} 
    };

    public bool loop = true;
    public bool invokable = true;

    public event Action TimeOver = null;
    
    public Timer(double time) => this.time = time;
    
    public double TimeElapsed {get; private set;} = 0;
    
    readonly double time;
    TimerPause timerPause = new();

    public void Update(double delta) {
        if (invokable == false) return;

        if (timerPause.paused) {
            timerPause.Update(delta);
            return;
        }

        TimeElapsed += delta;
        if (TimeElapsed > time) {
            // You can't invoke it again after it's already been invoked
            TimeOver?.Invoke();
            invokable = false;
            // Unless we are looping, in which case we can.
            if (loop) {
                TimeElapsed = 0;
                invokable = true;
            }
        } 
    }
    
    public void Pause(double timePaused) => timerPause = new(timePaused);

}

struct TimerPause {
    double pauseTime;
    public bool paused = false;
    double pauseTimeElapsed = 0;
    
    public TimerPause(double pauseTime) { 
        this.pauseTime = pauseTime;
        paused = true;
    }

    public void Update(double delta) {
        pauseTimeElapsed += delta;
        if (pauseTimeElapsed > pauseTime) {
            paused = false;
        }
    }
}
