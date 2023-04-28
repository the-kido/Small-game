using Godot;

public static class bloop {

	public static Vector2 PointAverage(this Rect2 rect) {
		float centerX = (rect.Position.X + rect.Size.X) / 2;
        float centerY = (rect.Position.Y + rect.Size.Y) / 2;
        return new(centerX, centerY);

	}
	public static int COOL(this int a) {
		return a + 1;
		//a.COOL();
	}
}