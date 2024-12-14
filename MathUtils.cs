using System;
using Godot;

[GlobalClass]
public partial class MathUtils : Godot.Node
{
    public static Vector2 GetNorthVector()
    {
        return new Vector2(0, -1);
    }
    
    public static Vector2I CubeToOddQ(Vector3I hex)
    {
        var q = hex.X;
        var r = hex.Y;

        var col = q;
        var row = r + ((q - (q & 1)) / 2);
        return new Vector2I(col, row);
    }

    public static Vector3I OddQToCube(Vector2I oddQ)
    {
        var col = oddQ.X;
        var row = oddQ.Y;

        var q = col;
        var r = row - ((col - (col & 1)) / 2);
        return new Vector3I(q, r, -q - r);
    }
}
