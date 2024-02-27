using Godot;

namespace Game.Bullets;

[GlobalClass]
public partial class BulletPatternResource : Resource {
	[Export(PropertyHint.Enum)]
	public BulletPattern.All pattern;
	[Export]
	public 	Godot.Collections.Array<BulletResource> bulletResources;
}
