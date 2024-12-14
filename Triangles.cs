using System;
using System.Collections.Generic;
using Godot;
using System.Linq;

public partial class Triangle : Node2D
{
    private Vertex[] _vertices;
    private Polygon2D _polygon = new Polygon2D();
    
    public Triangle()
        : this(new[] { new Vertex(), new Vertex(), new Vertex() })
    {
    }

    public Triangle(Vertex[] vertices)
    {
        _vertices = vertices;
    }

    public override void _Ready()
    {
        var firstVertexGlobalPosition = _vertices[0].GetGlobalPosition();
        List<Vector2> polygonPoints = new List<Vector2>();
        
        foreach (Vertex vertex in _vertices)
        {
            vertex.VertexClick += (vertex, height) =>
            {
                this.Update();
            };
            polygonPoints.Add(vertex.GetPosition());
        }

        // Triangle plane Vt is normal(Vector3(points))
        // Sun angle is Vs
        // sun reflection is Vsr = Vs.bounce(Vt)
        // camera vector is Vc = Vdown
        // triangle brightness B = Vsr dot Vc?

        double vectorX = 0;
        double vectorY = 0;
        double vectorZ = 0;

        //Vector3 sunAngleDegreesFromNorth = MathUtils.


        _polygon.SetPolygon(polygonPoints.ToArray());
        _polygon.SetColor(new Color(0, 0, 0, .5f));
        
        this.AddChild(_polygon);
    }

    private void Update()
    {
        
    }

    public Vector3 GetNormal()
    {
        return new Vector3(0, 0, 0);
    }
}

public partial class Triangles : Node
{
    private int _globalCount = 0;
    
    private Vector2 GetCirclePoint(int segmentIndex)
    {
        var angle = Math.PI / 3 * segmentIndex;
        var adjacent = (64 / 2) * Math.Cos((2 * Math.PI) - angle);
        var opposite = (64 / 2) * Math.Sin((2 * Math.PI) - angle);

        // Clamp
        if (opposite < -27)
        {
            opposite = -27;
        }

        if (opposite > 27)
        {
            opposite = 27;
        }

        return new Vector2((float)adjacent, (float)opposite);
    }

    private int RotatingIndex(int index, int count)
    {
        if (index == -1)
        {
            return count - 1;
        }

        if (index == -2)
        {
            return count - 2;
        }

        return index % count;
    }

    public override void _Ready()
    {
        var vertices = this.GetNode<Vertices>("/root/Root2D/TerrainSystem/Vertices");
        var triangles = new Node2D();

        var vertexDict = vertices.GetVertexDict();
        var centerVertices = vertices.GetCenterVertices();

        foreach (Vertex centerVertex in centerVertices)
        {
            var centerCoords = centerVertex.GetCoords();
            
            for (int i = 0; i < 6; i++)
            {
                var firstIndex = this.RotatingIndex(i, 6);
                var secondIndex = this.RotatingIndex(i + 1, 6);

                var firstCoords = Vertices.GetVertexFromCenter(centerCoords, firstIndex);
                var secondCoords = Vertices.GetVertexFromCenter(centerCoords, secondIndex);

                var firstVertex = vertices.GetVertex(firstCoords);
                var secondVertex = vertices.GetVertex(secondCoords);
                
                var triangle = new Triangle(new Vertex[]
                {
                    centerVertex,
                    firstVertex,
                    secondVertex,
                });

                this.AddChild(triangle);
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
