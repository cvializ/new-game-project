using Godot;

[GlobalClass]
public partial class CircleNode2D : Node2D
{
    private float _radius = 3f; 
   
    [Export]
    public float Radius {
        get { return _radius; }
        set {
            _radius = value;
            QueueRedraw();
        }
    }
    
    public override void _Draw()
    {
        this.DrawCircle(new Vector2(0f, 0f), Radius, new Color(1, 1, 1, 1));
    }
}
