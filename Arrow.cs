using Godot;
using System;

public partial class Arrow : Line2D
{
    public Arrow()
    {
        Width = 1;
        Points = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(15, 0),
            new Vector2(15, -8),
            new Vector2(25, 0),
            new Vector2(15, 8),
            new Vector2(15, 0),
        };
    }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
