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

    public Vertex GetDownhill()
    {
        return _vertices[0].GetHeight() >= _vertices[1].GetHeight() ? 
            _vertices[0] : 
            _vertices[1];
    }

    public override void _Ready()
    {
    }
}

public partial class Edges : Node
{
    public static Edges Instance;
    public Edges()
    {
        Instance = this;
    }
    
    private Godot.Collections.Dictionary<string, Edge> edgeDict = new Godot.Collections.Dictionary<string, Edge>();

    private void _MaybeAddEdge(Vertex startVertex, Vector4I endCoords)
    {
        Vector4I startCoords = startVertex.GetCoords();
        try
        {
            Vertex endVertex = Vertices.Instance.GetVertex(endCoords);
            Edge edge = new Edge(new Vertex[] { startVertex, endVertex });
            edgeDict[$"{startCoords}{endCoords}"] = edge;
            edgeDict[$"{endCoords}{startCoords}"] = edge;
            edge.SetGlobalPosition((startVertex.GetGlobalPosition() + endVertex.GetGlobalPosition()) / 2);
            
            AddChild(edge);
        }
        catch (Exception)
        {
            //GD.Print("No vertex", endCoords);
        }
    }
    
    public Edge GetEdge(Vertex start, Vertex end)
    {
        return edgeDict[$"{start.GetCoords()}{end.GetCoords}"];
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Iterate over vertices
        // Add edges of E, SE, SW.
//
        //foreach (Vertex vertex in Vertices.Instance.GetVertexDict().Values)
        //{
            //var coordsE = CoordinateUtils.TranslateVector(vertex.GetCoords(), 1, CoordinateUtils.Direction4E);
            //var coordsSE = CoordinateUtils.TranslateVector(vertex.GetCoords(), 1, CoordinateUtils.Direction4SE);
            //var coordsSW = CoordinateUtils.TranslateVector(vertex.GetCoords(), 1, CoordinateUtils.Direction4SW);
            //
            //_MaybeAddEdge(vertex, coordsE);
            //_MaybeAddEdge(vertex, coordsSE);
            //_MaybeAddEdge(vertex, coordsSW);
        //}
    }
}
