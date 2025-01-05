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
        List<Vector2> polygonPoints = new List<Vector2>();
        
        foreach (Vertex vertex in _vertices)
        {
            vertex.OnHeightChange += (vertex, height) =>
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
    public static Faces Instance;
    public Faces()
    {
        Instance = this;
    }
    
    private Godot.Collections.Dictionary<Vector3I, Face> _faceDict = new Godot.Collections.Dictionary<Vector3I, Face>();
   
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
        Vector4I origin = new Vector4I(0, 0, 0, 0);
        
        HeightMap heightMap = TerrainHeightMap.Instance.GetHeightMap();
        Vector2I size = heightMap.GetSize();
        
        
        // first row, Left faces
        for (int index = 0; index < size.X; index++)
        {
            var indexCoords = CoordinateUtils.TranslateVector(origin, index - 1, CoordinateUtils.Direction4E);
            var coordsSE = CoordinateUtils.TranslateVector(indexCoords, 1, CoordinateUtils.Direction4SE);
            var coordsE = CoordinateUtils.TranslateVector(indexCoords, 1, CoordinateUtils.Direction4E);
            
            try
            {           
                var face = new Face(new Vertex[]
                {
                    vertexDict[indexCoords],
                    vertexDict[coordsSE],
                    vertexDict[coordsE],
                });
                
                var vertexCoordsOddQ = CoordinateUtils.Vector4IToVector2I(indexCoords);
                var faceCoords = new Vector3I(vertexCoordsOddQ.X, vertexCoordsOddQ.Y, 0); // Left
                
                _faceDict[faceCoords] = face;
                AddChild(face);
            }
            catch (Exception)
            {
                // One of the vertices doesn't exist, no problem.
            }
        }
        
        // First row, right faces
        for (int index = 0; index < size.X; index++)
        {
            var indexCoords = CoordinateUtils.TranslateVector(origin, index - 1, CoordinateUtils.Direction4E);
            var coordsSE = CoordinateUtils.TranslateVector(indexCoords, 1, CoordinateUtils.Direction4SE);
            // TODO: something is broken about translating by SW
            var indexMinusOneCoords = CoordinateUtils.TranslateVector(origin, index - 2, CoordinateUtils.Direction4E);            
            var coordsSW = CoordinateUtils.TranslateVector(indexMinusOneCoords, 1, CoordinateUtils.Direction4SE);
            GD.Print("Right Index Coords ", indexCoords);
            GD.Print("Right SE Coords ", coordsSE);
            GD.Print("Right SW Coords ", coordsSW);
            
            try
            {           
                var face = new Face(new Vertex[]
                {
                    vertexDict[indexCoords],
                    vertexDict[coordsSE],
                    vertexDict[coordsSW],
                });
                
                var vertexCoordsOddQ = CoordinateUtils.Vector4IToVector2I(indexCoords);
                var faceCoords = new Vector3I(vertexCoordsOddQ.X, vertexCoordsOddQ.Y, 1); // Right
                
                _faceDict[faceCoords] = face;
                AddChild(face);
            }
            catch (Exception)
            {
                // One of the vertices doesn't exist, no problem.
            }
        }
        
        //var centerVertices = vertices.GetCenterVertices();
//
        ////int max = 0;
        //foreach (Vertex centerVertex in centerVertices)
        //{
            //var centerCoords = centerVertex.GetCoords();
            //
            //for (int i = 0; i < 6; i++)
            //{
                //var firstIndex = this.RotatingIndex(i, 6);
                //var secondIndex = this.RotatingIndex(i + 1, 6);
//
                //var firstCoords = Vertices.GetVertexFromCenter(centerCoords, firstIndex);
                //var secondCoords = Vertices.GetVertexFromCenter(centerCoords, secondIndex);
//
                //Vertex firstVertex, secondVertex;
                //try
                //{
                    //firstVertex = vertices.GetVertex(firstCoords);
                    //secondVertex = vertices.GetVertex(secondCoords);
                //}
                //catch (Exception)
                //{
                    ////GD.Print("Exception!", firstCoords, secondCoords);
                    //continue;
                //}
                //var face = new Face(new Vertex[]
                //{
                    //centerVertex,
                    //firstVertex,
                    //secondVertex,
                //});
                //
                //// q, r, L/R
                //Vector3I faceCoords = new Vector3I(0, 0, 0);
                //
                //_faceDict[faceCoords] = face;
//
                //this.AddChild(face);
                //
                ////if (max++ < 10)
                ////{
                    ////face.Print();
                ////}
            //}
        //}
    }
}
