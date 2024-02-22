using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Door : StaticBody2D {
	[Export]
	AnimationPlayer doorAnimation;
	
	public void Open() {
		doorAnimation.Play("Open");
	}
	
	public void InstantlyOpen() {
		doorAnimation.Play("Open", fromEnd: true);
		doorAnimation.Stop();
	}

	public override void _Ready() {
		KidoUtils.ErrorUtils.AvoidNullExportedVariables(doorAnimation, this);

		if (!doorAnimation.GetAnimationList().Contains("Open")) throw new KeyNotFoundException("There is no \"Open\" animation on the door " + Name +"!");
	}
}
