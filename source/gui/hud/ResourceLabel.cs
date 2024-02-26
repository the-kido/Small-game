using Godot;

namespace Game.Data;

// The label is a TextueRect so that it would expand the VBox easier
public partial class ResourceLabel : TextureRect {
	[Export]
	RichTextLabel label;
	[Export]
	AnimationPlayer animationPlayer;

	// temp
	public void SetText(string text, bool? valueIncreased = null) {
		label.Text = text;
		var thing = animationPlayer.GetAnimation("value_changed");

		if (valueIncreased is not null) {
			thing.TrackSetKeyValue(0, 0, (valueIncreased ?? true) ? gained : reduced);
			animationPlayer.Play("value_changed");
		}
	}

	public void PlayAnimation() {
		animationPlayer.Play();
	}

	Color reduced = new(1f, 0.5f, 0.5f);
	Color gained = new(1f, 0.84f, 0.5f);
}
