using System;
using System.Collections.Generic;
using Godot;
using System.Linq;

//
// Face data
// 
// soil type? tile layer
// normal, slope
// edges
// rate of flow
// rate of evaporation
// rate of infiltration
// rate of 

public partial class Face : Node2D
{
    private Vector3 _sunVector = new Vector3(0, -1, (float)Math.Sin(Math.PI / 3));
    
    private Vertex[] _vertices;
    private Polygon2D _polygon = new Polygon2D();
    
    public Face()
        : this(new[] { new Vertex(), new Vertex(), new Vertex() })
    {
    }

    public Face(Vertex[] vertices)
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
            Vector2 position = vertex.GetPosition();
            polygonPoints.Add(position);
        }

        _polygon.SetPolygon(polygonPoints.ToArray());
    
        _Update();
        
        this.AddChild(_polygon);
    }

    private Vector3 _GetNormal()
    {
        var A = new Vector3(_polygon.Polygon[0].X, _polygon.Polygon[0].Y, _vertices[0].GetHeight());
        var B = new Vector3(_polygon.Polygon[1].X, _polygon.Polygon[1].Y, _vertices[1].GetHeight());
        var C = new Vector3(_polygon.Polygon[2].X, _polygon.Polygon[2].Y, _vertices[2].GetHeight());
        var BA = B - A;
        var BC = B - C;
        var faceNormalVector = BA.Cross(BC).Normalized();
        
        return faceNormalVector;
    }

    private void _Update()
    {   
        var faceNormalVector = _GetNormal();
        var bounceVector = _sunVector.Bounce(faceNormalVector).Normalized();
        var cameraVector = new Vector3(0, 0, -1);
    
        var brightness = bounceVector.Dot(cameraVector);
        
        _polygon.SetColor(new Color(0, 0, 0, 1 - brightness));
    }

    public double GetSlope()
    {
        var referenceNormalVector = new Vector3(0, 0, 1);
        var normalVector = _GetNormal();
        double angle = referenceNormalVector.AngleTo(normalVector);
        double slope = Math.Tan(angle);
        
        return slope;
    }

    public Vector2 GetDownhillDirection()
    {
        var normalVector = _GetNormal();
        // what clock direction is the downward angle?
        // I guess it's the normal's  X,Y values
        Vector2 direction = new Vector2(normalVector.X, normalVector.Y);
        return direction.Normalized();
    }
    
    public void Print()
    {
        GD.Print("Face Start");
        GD.Print("Vertices");
        _vertices[0].Print();
        _vertices[1].Print();
        _vertices[2].Print();
        GD.Print("Normal", _GetNormal());
        GD.Print("Downhill", GetDownhillDirection());
        GD.Print("Slope", GetSlope());
        GD.Print("Face End");
    }
}

public partial class Faces : Node
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
        var faces = new Node2D();

        var vertexDict = vertices.GetVertexDict();
        var centerVertices = vertices.GetCenterVertices();

        int max = 0;
        foreach (Vertex centerVertex in centerVertices)
        {
            var centerCoords = centerVertex.GetCoords();
            
            for (int i = 0; i < 6; i++)
            {
                var firstIndex = this.RotatingIndex(i, 6);
                var secondIndex = this.RotatingIndex(i + 1, 6);

                var firstCoords = Vertices.GetVertexFromCenter(centerCoords, firstIndex);
                var secondCoords = Vertices.GetVertexFromCenter(centerCoords, secondIndex);

                Vertex firstVertex, secondVertex;
                try
                {
                    firstVertex = vertices.GetVertex(firstCoords);
                    secondVertex = vertices.GetVertex(secondCoords);
                }
                catch (Exception)
                {
                    continue;
                }
                var face = new Face(new Vertex[]
                {
                    centerVertex,
                    firstVertex,
                    secondVertex,
                });

                this.AddChild(face);
                
                if (max++ < 10)
                {
                    face.Print();
                }
            }
        }
    }
}
