using Godot;
using System;

public static class Extensions {

	public static Vector2 PointAverage(this Rect2 rect) {

		float centerX = (rect.Position.X + rect.Size.X) / 2;
        float centerY = (rect.Position.Y + rect.Size.Y) / 2;
        return new(centerX, centerY);
	}
	public static Vector2 Random(this Vector2 vector2) {
		Random random = new Random();
		float x = random.NextSingle() - 0.5f;
		float y = random.NextSingle() - 0.5f;
		
		return new Vector2(x, y).Normalized();

	}

	public static void ToggleYSorting(this Node2D node2D) {
		node2D.ZAsRelative = false;
		node2D.YSortEnabled = true;
	}

	public static string Colored(this string @string, string hexColor) =>
        $"[color={hexColor}]{@string}[/color]";
}