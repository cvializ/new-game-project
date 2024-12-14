using System;
using System.Collections.Generic;
using Godot;
using System.Linq;

public partial class Triangle : Node2D
{
    private Vertex[] vertices;
    private Node2D label;
    private Polygon2D polygon;
    
    public Triangle()
        : this(new[] { new Vertex(), new Vertex(), new Vertex() })
    {
    }

    public Triangle(Vertex[] vertices)
    {
        this.vertices = vertices;
    }

    public override void _Ready()
    {
        for (int i = 0; i < 3; i++)
        {
            this.vertices[i].VertexClick += () =>
            {
                this.Update();
            };
        }

        List<Vector2> polygonPoints = new List<Vector2>();


        var firstVertex = this.vertices[0];
        var secondVertex = this.vertices[1];
        var thirdVertex = this.vertices[2];

        var firstVertexGlobalPosition = firstVertex.GetGlobalPosition();
        
        var firstVertexLocalPosition = firstVertex.GetGlobalPosition() - firstVertexGlobalPosition;
        var secondVertexLocalPosition = secondVertex.GetGlobalPosition() - firstVertexGlobalPosition;
        var thirdVertexLocalPosition = thirdVertex.GetGlobalPosition() - firstVertexGlobalPosition;

        polygonPoints.Add(firstVertex.GetPosition());
        polygonPoints.Add(secondVertex.GetPosition());
        polygonPoints.Add(thirdVertex.GetPosition());

        var pointsArray = polygonPoints.ToArray();

        var polygon = new Polygon2D();
        polygon.SetPolygon(pointsArray);
        polygon.SetColor(new Color(0, 0, 0, .5f));
        
        this.AddChild(polygon);
    }

    private void Update()
    {
        GD.Print("UPDATE");
        //this.RemoveChild(this.label);
        ////this.label = LabelUtils.CreateLabel("T");
        //this.AddChild(this.label);
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
