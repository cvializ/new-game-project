using Godot;

[GlobalClass]
public partial class CircleNode2D : Node2D
{
	public override void _Draw()
	{
		this.DrawCircle(new Vector2(0f, 0f), 3f, new Color(1, 1, 1, 1));
	}	
}
