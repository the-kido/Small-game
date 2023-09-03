using System;
using Godot;

namespace KidoUtils;

public struct Timer {

    public static readonly Timer NONE = new() { 
        timerPause = new() {paused = true} 
    };

    readonly double time;
    
    readonly bool loop = true;
    bool invokable = true;
    
    readonly int? cycles = null;
    int cyclesDone = 0;

    public event Action TimeOver = null;
    public event Action AllLoopsFinished = null;
    
    public Timer(double time, bool loop = true, int cycles = -1) {
        this.time = time;
        this.loop = loop;  
        this.cycles = cycles;
    }
    
    public double TimeElapsed {get; private set;} = 0;

    TimerPause timerPause = new();

    public void Update(double delta) {
        if (invokable is false) 
            return;

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
                // Stuff required incase we want to loop the timer
                if (cycles is not null) {
                    if (cyclesDone < cycles)
                        cyclesDone += 1;

                    if (cyclesDone == cycles) {
                        AllLoopsFinished?.Invoke();
                        invokable = false;
                        return;
                    }
                }
                
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
