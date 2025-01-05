using Godot;

[GlobalClass, Tool]
public partial class CircleNode2D : Node2D
{
    private float _radius = 5f; 
   
    [Export]
    public float Radius {
        get { return _radius; }
        set {
            _radius = value;
            QueueRedraw();
        }
    }
    
    public override void _Ready()
    {
        Transform = ((Node2D)GetParent()).Transform.AffineInverse() * Transform;
    }
    
    public override void _Draw()
    {
        DrawCircle(new Vector2(0f, 0f), Radius, new Color(1, 1, 1, 1));
    }
}
