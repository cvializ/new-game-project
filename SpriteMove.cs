using System;
using Godot;

public partial class SpriteMove : Sprite2D
{
    private float speed = 400;
    private float angularSpeed = Mathf.Pi / 2;

    public override void _Ready()
    {
        var timer = this.GetNode<Timer>("Timer");
        timer.Timeout += this.OnTimerTimeout;
    }

    public override void _Process(double delta)
    {
        this.Rotation += this.angularSpeed * (float)delta;
        var velocity = Vector2.Up.Rotated(this.Rotation) * this.speed;
        this.Position += velocity * (float)delta;
    }

    // We also specified this function name in PascalCase in the editor's connection window.
    private void OnButtonPressed()
    {
        GD.Print("wow");
        this.SetProcess(!this.IsProcessing());
    }

    private void OnTimerTimeout()
    {
        this.Visible = !this.Visible;
    }
}
