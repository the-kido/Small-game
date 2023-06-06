using Godot;
using System;

public partial class OnPlayerIndicator : Control {
    [Export]
    ProgressBar progressBar;

    

    public void SetProgress(int value) {
        progressBar.Value = value;
    }

}
