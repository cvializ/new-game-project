using Godot;
using System;

[Tool]
public partial class TestSkew : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //float skewAngle = (float)(Math.PI / 3);
        //Vector2 xAxis = new Vector2(1, 0);
        ////Vector2 yAxis = new Vector2(1, 0).Rotated(skewAngle);
        //Vector2 scale = new Vector2(1, 1);
        ////Vector2 position = new Vector2(, 32);
        //
        //Vector2 sideAngle = new Vector2(1, 0).Rotated(-skewAngle);
        //float length = sideAngle.Dot(xAxis);
        //Vector2 position = new Vector2(length, 0);
        //
        //Transform2D hexTransform = new Transform2D(0, scale, skewAngle, position);
        //
        //GD.Print("SKEW: ", hexTransform.Skew);
        //
        //SetTransform(hexTransform);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
