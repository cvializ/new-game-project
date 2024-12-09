using Godot;
using System;

public partial class SpriteMove : Sprite2D
{
	private float _speed = 400;
	private float _angularSpeed = Mathf.Pi / 2;

	public override void _Ready()
	{
		var timer = GetNode<Timer>("Timer");
		timer.Timeout += OnTimerTimeout;
	}

	public override void _Process(double delta)
	{
		Rotation += _angularSpeed * (float)delta;
		var velocity = Vector2.Up.Rotated(Rotation) * _speed;
		Position += velocity * (float)delta;
	}

	// We also specified this function name in PascalCase in the editor's connection window.
	private void OnButtonPressed()
	{
		GD.Print("wow");
		SetProcess(!IsProcessing());
	}
	
	private void OnTimerTimeout()
	{
		Visible = !Visible;
	}
}
