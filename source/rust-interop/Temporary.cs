using Godot;

using System.Runtime.InteropServices;


// I'm trying something here so give me a break
public static class Temp {
    [DllImport("Ug.dll")]
    public static extern int add(int a, int b);

    public static void Work() {
        GD.Print("NOWAY: ", add(3, 4));
    }
}