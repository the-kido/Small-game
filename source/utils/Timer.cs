using System;

// w.i.p
struct Timer {
    public bool loop = true;
    public event Action TimeOver = null;
    public Timer(double time) => this.time = time;
    
    double time;
    double timeElapsed = 0;
    TimerPause timerPause = new();

    public void Update(double delta) {
        if (timerPause.paused) {
            timerPause.Update(delta);
            return;
        }

        timeElapsed += delta;
        if (timeElapsed > time) {
            TimeOver?.Invoke();
            if (loop) timeElapsed = 0;
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
