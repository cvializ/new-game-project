using System;
using Godot;

[GlobalClass]
public partial class MathUtils : Godot.Node
{
    public static Vector2 GetNorthVector()
    {
        return new Vector2(0, -1);
    }
    
    public static Vector2I CubeToAxial(Vector3I hex)
    {
        return new Vector2I(hex.X, hex.Y);
    }
    
    public static Vector3I AxialToCube(Vector2I hex)
    {
        var q = hex.X;
        var r = hex.Y;
        var s = -q - r;
        return new Vector3I(q, r, s);
    }
    
    public static Vector2I AxialToOddQ(Vector2I hex)
    {
        var col = hex.X;
        var row = hex.Y + (hex.X - (hex.X&1)) / 2;
        return new Vector2I(col, row);
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
    
    public static Vector2I OddQToAxial(Vector2I hex)
    {
        var q = hex.X;
        var r = hex.Y - (hex.X - (hex.X&1)) / 2;
        return new Vector2I(q, r);
    }
    
    public static Vector2I VertexCoordsToGridCoord(Vector4I vertexCoords)
    {
        return new Vector2I(0, 0);
    }
}
