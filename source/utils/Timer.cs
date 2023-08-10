using System;

// w.i.p
public struct Timer {
    public bool loop = true;
    public event Action TimeOver = null;
    public Timer(double time) => this.time = time;
    public double TimeElapsed {get; private set;} = 0;
    
    double time;
    TimerPause timerPause = new();

    public void Update(double delta) {
        if (timerPause.paused) {
            timerPause.Update(delta);
            return;
        }

        TimeElapsed += delta;
        if (TimeElapsed > time) {
            TimeOver?.Invoke();
            if (loop) TimeElapsed = 0;
        } 
    }
    
    public void Pause(double timePaused) => timerPause = new(timePaused);

}

struct TimerPause {
    double pauseTime;
    public bool paused = true;
    double pauseTimeElapsed = 0;
    
    public TimerPause(double pauseTime) { 
        this.pauseTime = pauseTime;
    }

    public void Update(double delta) {
        pauseTimeElapsed += delta;
        if (pauseTimeElapsed > pauseTime) {
            paused = false;
        }
    }
}
