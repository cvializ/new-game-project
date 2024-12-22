using System;
using System.Collections.Generic;
using Godot;
using System.Linq;

public partial class Triangle : Node2D
{
    private Vector3 SunVector = new Vector3(0, -1, (float)Math.Sin(Math.PI / 3));
    
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
                this._Update();
            };
            polygonPoints.Add(vertex.GetPosition());
        }

        GD.Print("We polygonnin");

        _polygon.SetPolygon(polygonPoints.ToArray());
    
        _Update();
        
        this.AddChild(_polygon);
    }

    private void _Update()
    {   
        var A = new Vector3(_polygon.Polygon[0].X, _polygon.Polygon[0].Y, _vertices[0].GetHeight());
        var B = new Vector3(_polygon.Polygon[1].X, _polygon.Polygon[1].Y, _vertices[1].GetHeight());
        var C = new Vector3(_polygon.Polygon[2].X, _polygon.Polygon[2].Y, _vertices[2].GetHeight());
        var BA = B - A;
        var BC = B - C;
        var trianglePlaneNormalVector = BA.Cross(BC).Normalized();
        
        var bounceVector = SunVector.Bounce(trianglePlaneNormalVector).Normalized();
        var cameraVector = new Vector3(0, 0, -1);
    
        var brightness = bounceVector.Dot(cameraVector);
        
        _polygon.SetColor(new Color(0, 0, 0, 1 - brightness));
        
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
            GD.Print("centerVertex ", centerCoords);
            
            for (int i = 0; i < 6; i++)
            {
                var firstIndex = this.RotatingIndex(i, 6);
                var secondIndex = this.RotatingIndex(i + 1, 6);

                var firstCoords = Vertices.GetVertexFromCenter(centerCoords, firstIndex);
                var secondCoords = Vertices.GetVertexFromCenter(centerCoords, secondIndex);

                GD.Print(firstCoords, secondCoords);

                Vertex firstVertex, secondVertex;
                try
                {
                    firstVertex = vertices.GetVertex(firstCoords);
                    secondVertex = vertices.GetVertex(secondCoords);
                }
                catch (Exception e)
                {
                    GD.Print("oops");
                    continue;
                }
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
