using Godot;
using System;


public partial class Edge : Node2D 
{
    private Vertex[] _vertices;
    
    public Edge()
        : this(new[] { new Vertex(), new Vertex() })
    {
    }

    public Edge(Vertex[] vertices)
    {
        _vertices = vertices;
    }

    public override void _Ready()
    {
    }
}

public partial class Edges : Node
{
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
