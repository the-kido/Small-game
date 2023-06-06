using Godot;
using System.Collections.Generic;

// How can I make easily referenced portraits that I can use? 
// I only need a handful for a handful of characters. Then, they should be statically referable by
// any class that wishes to use the portrait, whether animated or not.
public struct Portraits {
    public static Portrait None = new(null, "");
    static SpriteFrames bossSprites() {
        SpriteFrames temp = ResourceLoader.Load<SpriteFrames>("res://assets/portraits/first_boss/first_boss.tres"); 

        return temp;
    }
    public static Dictionary<string, Portrait> boss = new() {
        {"Happy", new(bossSprites(), "happy") {loop = true} },
        {"Sad", new(bossSprites(), "sad") }

    };
}