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

    private Vector2 _GetMovementVectorFromInput()
    {
        var movementVector = new Vector2(0, 0);
        
        if (Input.IsActionPressed("camera_up"))
        {
            movementVector += new Vector2(0, -1);
        }
        if (Input.IsActionPressed("camera_right"))
        {
            movementVector += new Vector2(1, 0);
        }
        if (Input.IsActionPressed("camera_down"))
        {
            movementVector += new Vector2(0, 1);
        }
        if (Input.IsActionPressed("camera_left"))
        {
            movementVector += new Vector2(-1, 0);
        }
        
        // Going diagonal doesn't make you go faster : )
        return movementVector.Normalized();
    }
    
    private int _GetZoomChangeFromInput()
    {
        if (Input.IsActionPressed("camera_zoom_in"))
        {
            return 1;
        }
        
        if (Input.IsActionPressed("camera_zoom_out"))
        {
            return -1;
        }
        
        return 0;
    }
    
    private void _UpdateZoom(double zoom)
    {
        var deltaVector = new Vector2((float)zoom, (float)zoom);
        var nextZoom = Zoom + deltaVector;
        
        if (nextZoom[0] <= 0.5) {
            return;
        }
        
        if (nextZoom[0] >= 2)
        {
            return;
        }
        
        Zoom = nextZoom;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        float magnitude = (float)(_speed * delta);
        GlobalTranslate(magnitude * _GetMovementVectorFromInput());
        _UpdateZoom(_GetZoomChangeFromInput() * delta);
    }
}
