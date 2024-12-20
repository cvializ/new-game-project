using System;
using Godot;

//delta W
//3 0
//2 2
//1 1
//0 0
//-1 2
//-2 1
//-3 0
//-4 2

[GlobalClass]
public partial class CoordinateUtils : Godot.Node
{
    public static Vector4I WUnitVector = new Vector4I(0, 0, 0, 1);
    
    public static Vector4I RotateW(Vector4I vector, int max = 3)
    {
        //return (new Vector4I(0, 0, 0, 1) * vector + new Vector4I(0, 0, 0, 3)) % 3;
        var xyzVector = new Vector4I(1, 1, 1, 0) * vector;
        var wVector = WUnitVector * vector;
        
        if (wVector.W >= 0)
        {
            return wVector % 3;
        }
        
        var rotatedWVector = ((wVector - 2 * WUnitVector) % 3) + 2 * WUnitVector;
        
        return xyzVector + rotatedWVector;
    }
    public static Vector4I UnitVector4 = new Vector4I(0, 0, 0, -1);

    
    public static Vector4I CarryVector4 = new Vector4I(-1, -1, 2, 0);
    
    public static Vector4I Direction4NW = new Vector4I(0, 0, 0, -1);
    public static Vector4I Direction4SE = -Direction4NW;
    public static Vector4I Direction4SW = new Vector4I(0, 1, -1, -1);
    public static Vector4I Direction4NE = -Direction4SW;
    public static Vector4I Direction4E = new Vector4I(1, 0, -1, -1);
    public static Vector4I Direction4W = -Direction4E;

    public static Vector2I Direction2NW = new Vector2I(0, -1);
    public static Vector2I Direction2SE = -Direction2NW;
    public static Vector2I Direction2SW = new Vector2I(-1, 1);
    public static Vector2I Direction2NE = -Direction2SW;
    public static Vector2I Direction2E = new Vector2I(1, 0);
    public static Vector2I Direction2W = -Direction2E;
    
    
    
    public static Vector4I TranslateVector(Vector4I initial, int delta, Vector4I direction)
    {
        GD.Print("TranslateVector");
        int carryCount = GetCarryCount(initial.W, Math.Sign(direction.W) * delta);
        GD.Print("carries", carryCount);
        GD.Print("initial", initial);
        GD.Print("direction", direction);
        GD.Print("delta", delta);
        
        var carryVector = CarryVector4 * carryCount;
        GD.Print("carryVector", carryVector);
        
        //GD.Print("direction * delta", );
        //
        ////var w = vectorW.W;
        ////var carries = (w - ((w + 3) % 3)) / 3;
        ////GD.Print($"w {w}");
        ////GD.Print($"carries {carries}");
        ////Vector4I result = initial + (UnitVector4 * delta) + direction * carries;
        ////return RotateW(result);
        //
        //return RotateW(resultNoCarry) + direction * carries; // - CarryVector4;
        //if (carries < 0)
        //{
            //direction = -direction;
        //}
        
        return RotateW(initial + (direction * delta) + carryVector);
        
        
    }
    
    public static Vector2I TranslateVector(Vector2I initial, int delta, Vector2I direction)
    {
        return initial + delta * direction;
    }
    
    public static Vector4I Vector2IToVector4I(Vector2I initial)
    {
        int q = (int)(initial * Direction2E).X;
        int r = (int)(initial * Direction2SE).Y;
        
        GD.Print($"(q, r) = ({q}, {r})");
        
        Vector4I unitsE = TranslateVector(new Vector4I(0, 0, 0, 0), q, Direction4E);
        Vector4I unitsSE = TranslateVector(new Vector4I(0, 0, 0, 0), r, Direction4SE);
        
        return unitsE + unitsSE;
    }
    
    private static int GetCarryCount(int w_i, int delta)
    {        
        GD.Print($"GetCarryCount({w_i}, {delta})");
        int carry = (int)Math.Floor((decimal)((Math.Abs(delta) + (w_i % 3)) / 3));
        if (delta < 0)
        {
            
            return -(int)Math.Floor((decimal)((Math.Abs(delta - 2) + (w_i % 3)) / 3));
        }
        
        return carry;
    }
    
    public override void _Ready()
    {
        //GD.Print("WOW", Vector2IToVector4I(new Vector2I(2, 1)));
        
        GD.Print("(0, 0, 0, 0) NW 1", TranslateVector(new Vector4I(0, 0, 0, 0), 1, Direction4NW));
        GD.Print("(0, 0, 0, 0) NW 2", TranslateVector(new Vector4I(0, 0, 0, 0), 2, Direction4NW));
        GD.Print("(0, 0, 0, 0) NW 3", TranslateVector(new Vector4I(0, 0, 0, 0), 3, Direction4NW));
        GD.Print("(0, 0, 0, 0) NW 4", TranslateVector(new Vector4I(0, 0, 0, 0), 4, Direction4NW));
        
        GD.Print("(0, 0, 0, 0) SE 1", TranslateVector(new Vector4I(0, 0, 0, 0), 1, Direction4SE));
        GD.Print("(0, 0, 0, 0) SE 2", TranslateVector(new Vector4I(0, 0, 0, 0), 2, Direction4SE));
        GD.Print("(0, 0, 0, 0) SE 3", TranslateVector(new Vector4I(0, 0, 0, 0), 3, Direction4SE));
        GD.Print("(0, 0, 0, 0) SE 4", TranslateVector(new Vector4I(0, 0, 0, 0), 4, Direction4SE));
        //GD.Print("(0, 0, 0, 1)", TranslateVector(new Vector4I(0, 0, 0, 0), 1, Direction4NE));
        
        //GD.Print(Vector2IToVector4I(new Vector2I(-2, 2)));
        //GD.Print(TranslateVector(new Vector2I(1, 0), 1, Direction2SE));
        GD.Print("XXX", TranslateVector(new Vector4I(0, 0, 0, 0), 1, Direction4E));
        
        for (int w = 0; w < 10; w++)
        {
            GD.Print($"RotateW {-5 + w} ", RotateW(new Vector4I(0, 0, 0, -5 + w)));
            for (int d = 0; d < 10; d++)
            {
                //var carry = GetCarryCount(w, -5 + d);
                //GD.Print($"w + d = carry {w} + {-5 + d} = {carry}");
            }
            GD.Print("");
        }
    }
}
