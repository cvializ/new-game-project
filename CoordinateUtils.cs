using System;
using Godot;

[GlobalClass]
public partial class CoordinateUtils : Godot.Node
{
    public static Vector4I WUnitVector = new Vector4I(0, 0, 0, 1);
    
    public static Vector4I RotateW(Vector4I vector)
    {
        var xyzVector = new Vector4I(1, 1, 1, 0) * vector;
        var wVector = WUnitVector * vector;
        
        if (wVector.W >= 0)
        {
            return xyzVector + (wVector % 3);
        }
        
        // -1, -2, -3, ... => 0 2 1 0 2 1 0
        // -3, -3, -4, ... =>
        // 0, -1, -2, 0, -1, ... =>
        // 2, 1, 0, 2, 1, 0
        var rotatedWVector = ((wVector - 2 * WUnitVector) % 3) + 2 * WUnitVector;
        
        return xyzVector + rotatedWVector;
    }
    
    public static Vector4I CarryVector4 = new Vector4I(-1, -1, 2, 0);
    
    // Axis
    public static Vector4I Direction4NW = new Vector4I(0, 0, 0, -1);
    public static Vector4I CarryNW = CarryVector4;
    public static Vector4I Direction4SE = -Direction4NW;
    public static Vector4I CarrySE = -CarryVector4;
    
    // Axis
    public static Vector4I Direction4SW = new Vector4I(0, 1, -1, -1);
    public static Vector4I CarrySW = CarryVector4;
    public static Vector4I Direction4NE = -Direction4SW;
    public static Vector4I CarryNE = -CarryVector4;
    
    // Axis
    public static Vector4I Direction4E = new Vector4I(1, 0, -1, -1);
    public static Vector4I CarryE = CarryVector4;
    public static Vector4I Direction4W = -Direction4E;
    public static Vector4I CarryW = -CarryVector4;
    
    // Vector2
    public static Vector2I Direction2NW = new Vector2I(0, -1);
    public static Vector2I Direction2SE = -Direction2NW;
    public static Vector2I Direction2SW = new Vector2I(-1, 1);
    public static Vector2I Direction2NE = -Direction2SW;
    public static Vector2I Direction2E = new Vector2I(1, 0);
    public static Vector2I Direction2W = -Direction2E;
    
    public static Vector4I TranslateVector(Vector4I initial, int delta, Vector4I direction)
    {
        Vector4I carry;
        switch (direction)
        {
            // Axis
            case var value when value == Direction4NW:
                carry = CarryNW;
                break;
            case var value when value == Direction4SE:
                carry = CarrySE;
                break;
                
            // Axis
            case var value when value == Direction4SW:
                carry = CarrySW;
                break;
            case var value when value == Direction4NE:
                carry = CarryNE;
                break;
                
            // Axis
            case var value when value == Direction4E:
                carry = CarryE;
                break;
            case var value when value == Direction4W:
                carry = CarryW;
                break;
                
            // Oopsis
            default:
                carry = new Vector4I(-999, -999, -999, -999);
                break;
        }
        var normalizedDelta = Math.Abs(delta);
        var normalizedCarry = Math.Sign(delta) * carry;
        
        int carryCount = Math.Abs(GetCarryCount(initial.W, Math.Sign(direction.W) * delta));
        var carryVector = normalizedCarry * carryCount;
        
        return RotateW(initial + (direction * delta) + carryVector);
    }
    
    public static Vector2I TranslateVector(Vector2I initial, int delta, Vector2I direction)
    {
        return initial + delta * direction;
    }

    public static Vector2I PixelToVertexAxialCoords(Vector2I pixelCoords)
    {
        int row = pixelCoords.Y;
        int column = pixelCoords.X;
        
        // Handle side sawtooth
        Vector2I evenRowDelta = new Vector2I(0, 1);
        Vector2I oddRowDelta = new Vector2I(-1, 1);
        int pairs = row / 2;
        
        // Handle top sawtooth
        Vector2I thirdOfFourColumnsDelta = row % 2 == 0 ? new Vector2I(-1, 1) : new Vector2I(0, 1);
        var adjustment = (column - 2) % 3 == 0 ? thirdOfFourColumnsDelta : new Vector2I(0, 0);
        var next = pairs * (oddRowDelta + evenRowDelta) + (row % 2 == 0 ? new Vector2I(0, 0) : oddRowDelta);
        
        return next + new Vector2I(1, 0) * pixelCoords + adjustment;
    }
    
    public static Vector4I Vector2IToVector4I(Vector2I initial)
    {
        int q = (int)(initial * Direction2E).X;
        int r = (int)(initial * Direction2SE).Y;
        
        //GD.Print($"(q, r) = ({q}, {r})");
        
        Vector4I initialVector = new Vector4I(0, 0, 0, 0);
        Vector4I vectorTranslatedE = TranslateVector(initialVector, q, Direction4E);
        Vector4I vectorTranslatedEThenSE = TranslateVector(vectorTranslatedE, r, Direction4SE);
            
        return vectorTranslatedEThenSE;
    }
    
    public static Vector2I Vector4IToVector2I(Vector4I initial)
    {
        Vector3I hexCubeCoords = new Vector3I(initial.X, initial.Y, initial.Z);
        Vector2I oddQCoords = MathUtils.CubeToOddQ(hexCubeCoords);

        return oddQCoords + (initial.W * Direction2SE);
    }
    
    private static int GetCarryCount(int w_i, int delta)
    {        
        //GD.Print($"GetCarryCount({w_i}, {delta})");
        int carry = (int)Math.Floor((decimal)((Math.Abs(delta) + (w_i % 3)) / 3));
        if (delta < 0)
        {
            
            return -(int)Math.Floor((decimal)((Math.Abs(delta - 2) + (w_i % 3)) / 3));
        }
        
        return carry;
    }
}
