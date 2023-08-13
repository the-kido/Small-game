using System;
using Godot;
using System.Threading;
using System.Threading.Tasks;


// [GlobalClass]
public abstract partial class LevelEvent : Node {    
    public abstract void Start();

    public abstract event Action Finished;
}