using Godot;
using System;

public partial class GameCamera : Camera2D
{
    [Export]
    private double _speed;

    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    private Vector2 _GetMovementVector()
    {
        var movementVector = new Vector2(0, 0);
        
        if (Input.IsActionPressed("ui_up"))
        {
            movementVector += new Vector2(0, -1);
        }
        if (Input.IsActionPressed("ui_right"))
        {
            movementVector += new Vector2(1, 0);
        }
        if (Input.IsActionPressed("ui_down"))
        {
            movementVector += new Vector2(0, 1);
        }
        if (Input.IsActionPressed("ui_left"))
        {
            movementVector += new Vector2(-1, 0);
        }
        
        // Going diagonal doesn't make you go faster : )
        return movementVector.Normalized();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        float magnitude = (float)(_speed * delta);
        GlobalTranslate(magnitude * _GetMovementVector());
    }
}
