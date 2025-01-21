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
    public static System.Random Random = new System.Random();
    
    private Vector3 _sunVector = new Vector3(0, -1, (float)Math.Sin(Math.PI / 3));
    
    private Vector3I _coords;
    private Vertex[] _vertices;
    private bool _hasWater = false;
    private Polygon2D _polygon = new Polygon2D();
    
    public Face()
        : this(new Vector3I(0, 0, 0), new[] { new Vertex(), new Vertex(), new Vertex() })
    {
    }

    public Face(Vector3I coords, Vertex[] vertices)
    {
        _coords = coords;
        _vertices = vertices;
    }
    
    public Vector3I GetNeighbor(Vector2 direction)
    {
        //GD.Print("Neighbor direction", direction);
        bool isLeftTriangle = _coords.Z == 0;
        
        var deltaN = new Vector3I(0, -1, 0);
        var deltaSE = new Vector3I(0, 0, 0);
        var deltaSW = new Vector3I(-1, 0, 0);
        
        var dirSE = Vector2.Right.Rotated((float)(Math.PI / 3));
        var dirN = Vector2.Right.Rotated((float)(-Math.PI / 2));
        var dirSW = Vector2.Right.Rotated((float)(2 * Math.PI / 3));
        
        if (isLeftTriangle)
        {
            var dirs = new Vector2[] {
                dirSE,
                dirN,
                dirSW,
            };
            var sorted = dirs.OrderBy(dir => direction.DistanceTo(dir)).ToArray();            
            
            if (sorted[0] == dirSE)
            {
                GD.Print("SE");
                return _coords + deltaSE + new Vector3I(0, 0, 1);
            }
            if (sorted[0] == dirN)
            {
                GD.Print("N");
                return _coords + deltaN + new Vector3I(0, 0, 1);
            }
            if (sorted[0] == dirSW)
            {
                GD.Print("SW");
                return _coords + deltaSW + new Vector3I(0, 0, 1);
            }
            
            return new Vector3I(-999, -999, -999);
        }
        
        //GD.Print("bottom");
        
        var deltaS = -deltaN;
        var deltaNW = -deltaSE;
        var deltaNE = -deltaSW;
        
        var dirNW = Vector2.Right.Rotated((float)(-2 * Math.PI / 3));
        var dirS = Vector2.Right.Rotated((float)(Math.PI / 2));
        var dirNE = Vector2.Right.Rotated((float)(-Math.PI / 3));
        
        var dirsBottom = new Vector2[] {
            dirNW,
            dirS,
            dirNE,
        };
        var sortedBottom = dirsBottom.OrderBy(dir => direction.DistanceSquaredTo(dir)).ToArray();
        
        if (sortedBottom[0] == dirNW)
        {
            GD.Print("NW");
            return _coords + deltaNW + new Vector3I(0, 0, -1);
        }
        if (sortedBottom[0] == dirS)
        {
            GD.Print("S");
            return _coords + deltaS + new Vector3I(0, 0, -1);
        }
        if (sortedBottom[0] == dirNE)
        {
            GD.Print("NE");
            return _coords + deltaNE + new Vector3I(0, 0, -1);
        }

        //GD.Print(direction.Angle());
        
        return new Vector3I(-999, -999, -999);
    }

    public void SetWater(bool hasWater)
    {
        _hasWater = hasWater;
        _Update();
    }
    
    public bool GetHasWater()
    {
        return _hasWater;
    }
    
    private double cumulativeDelta = 0;
    public override void _Process(double delta)
    {
        cumulativeDelta += delta;
        if (cumulativeDelta > 1)
        {
            Flow();
            cumulativeDelta = 0;
        }
    }
    
    public Vector3I GetCoords()
    {
        return _coords;
    }
    
    public void Flow()
    {
        if (!_hasWater)
        {
            return;
        }
        
                
        var downhill = GetDownhillDirection();
        var neighborCoords = GetNeighbor(downhill); // Is this working?
        
        GD.Print("FLOW DIRECTION ", downhill);
        
        var next = Faces.Instance.GetFace(neighborCoords);
        if (next.GetHasWater())
        {
            return;
        }
        
        SetWater(false);    
        next.SetWater(true);
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
        
        AddChild(_polygon);
        
    }
    
    public Vector2 GetCenter()
    {
        var polygonPoints = _polygon.Polygon;
        return new Vector2(
            (
                polygonPoints[0].X +
                polygonPoints[1].X +
                polygonPoints[2].X
            ) / 3, 
            (
                polygonPoints[0].Y +
                polygonPoints[1].Y +
                polygonPoints[2].Y
            ) / 3);
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

    private Arrow _arrow;

    private void _UpdateArrow()
    {
        //RemoveChild(_arrow);
        //
        //_arrow = new Arrow();
        //_arrow.Rotate(GetDownhillDirection().Angle());
        //_arrow.SetGlobalPosition(GetCenter());
        //
        //AddChild(_arrow);
    }

    private void _Update()
    {   
        var faceNormalVector = _GetNormal();
        var bounceVector = _sunVector.Bounce(faceNormalVector).Normalized();
        var cameraVector = new Vector3(0, 0, -1);
    
        var brightness = bounceVector.Dot(cameraVector);
        
        _polygon.SetColor(new Color(0, 0, _hasWater ? 1 : 0, 1 - brightness));
        
        _UpdateArrow();
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
        
        if (direction == new Vector2(0, 0))
        {
            float randomAngle = (float)(Math.PI * 2 * Face.Random.NextDouble());
            
            GD.Print("NORMAL (random)", normalVector);
            return Vector2.Right.Rotated(randomAngle).Normalized();
        }
        
        GD.Print("NORMAL", normalVector);
        return direction.Normalized();
    }
}

public partial class Faces : Node
{   
    [Signal]
    public delegate void FaceClickEventHandler(Face face);
    
    
    public static Faces Instance;
    public Faces()
    {
        Instance = this;
    }
    
    private Godot.Collections.Dictionary<Vector3I, Face> _faceDict = new Godot.Collections.Dictionary<Vector3I, Face>();

    public Face GetFace(Vector3I faceCoords)
    {
        try 
        {
            return _faceDict[faceCoords];            
        } 
        catch (Exception)
        {
            throw new Exception($"Could not find face at {faceCoords}");
        }
    }
    
    public Vector3I VertexToFace(Vector3I cellCubeCoords)
    {
        Vector2I axialCoords = MathUtils.CubeToAxial(cellCubeCoords);
        
        var y = axialCoords.Y * (CoordinateUtils.Direction2SE + CoordinateUtils.Direction2SW);
        var x = axialCoords.X * (CoordinateUtils.Direction2SE +CoordinateUtils.Direction2E);
        
        var combo = (x + y);
        
        return new Vector3I(combo.X, combo.Y, 0);
    }
    
    public Vector3I GetFaceFromCellVertex(Vector3I cellCubeCoords, double angle)
    {
        Vector3I faceCoords = VertexToFace(cellCubeCoords);
        Face face = GetFace(faceCoords);
        
        //var rotated = (angle + (4 * Math.PI / 3)) % (2 * Math.PI);
        //var vertex = (int)Math.Round(angle / (Math.PI / 3));
        if (angle < Math.PI / 3)
        {
            return face.GetNeighbor(Vector2.Right);
        }
        
        if (angle < 2 * Math.PI / 3)
        {
            return face.GetCoords();
        }
        
        if (angle < Math.PI)
        {
            return face.GetNeighbor(Vector2.Left);
        }
        
        if (angle < 4 * Math.PI / 3)
        {
            var leftNeighborCoords = face.GetNeighbor(Vector2.Left);
            var leftNeighbor = GetFace(leftNeighborCoords);
            
            var southNeighborOfleftNeighborCoords = leftNeighbor.GetNeighbor(Vector2.Down);
            return southNeighborOfleftNeighborCoords;
        }
        
        if (angle < 5 * Math.PI / 3)
        {
            
            var leftNeighborCoords = face.GetNeighbor(Vector2.Left);
            var leftNeighbor = GetFace(leftNeighborCoords);
            
            var southNeighborOfleftNeighborCoords = leftNeighbor.GetNeighbor(Vector2.Down);
            var southNeighborOfleftNeighbor = GetFace(southNeighborOfleftNeighborCoords);
            
            var goal = southNeighborOfleftNeighbor.GetNeighbor(Vector2.Right);
            
            return goal;
        }
        
        var rightNeighborCoords = face.GetNeighbor(Vector2.Right);
        var rightNeighbor = GetFace(rightNeighborCoords);
        
        var southNeighborOfRightNeighborCoords = rightNeighbor.GetNeighbor(Vector2.Down);
        return southNeighborOfRightNeighborCoords;
    }

    public override void _Ready()
    {
        var vertexDict = Vertices.Instance.GetVertexDict();
        
        HeightMap heightMap = TerrainHeightMap.Instance.GetHeightMap();
        Vector2I size = heightMap.GetSize();
        
        for (int row = 0; row < size.Y; row++)
        {
            var rowOrigin = CoordinateUtils.PixelToVertexAxialCoords(new Vector2I(0, row));
            
            // first row, Left faces
            for (int index = 0; index < size.X; index++)
            {
                var indexCoords = CoordinateUtils.TranslateVector(rowOrigin, index - 1, CoordinateUtils.Direction2E);
                var coordsSE = CoordinateUtils.TranslateVector(indexCoords, 1, CoordinateUtils.Direction2SE);
                var coordsE = CoordinateUtils.TranslateVector(indexCoords, 1, CoordinateUtils.Direction2E);
                
                try
                {           
                    var faceCoords = new Vector3I(indexCoords.X, indexCoords.Y, 0); // Left
                    var face = new Face(faceCoords, new Vertex[]
                    {
                        vertexDict[CoordinateUtils.Vector2IToVector4I(indexCoords)],
                        vertexDict[CoordinateUtils.Vector2IToVector4I(coordsSE)],
                        vertexDict[CoordinateUtils.Vector2IToVector4I(coordsE)],
                    });
                    
                    _faceDict[faceCoords] = face;
                    //GD.Print("FACE COORDS, ", faceCoords);
                    
                    AddChild(face);
                }
                catch (Exception)
                {
                    // One of the vertices doesn't exist, no problem.
                }
            }
            
            //var max = 0;
            // first row, Right faces
            for (int index = 0; index < size.X; index++)
            {
                //if (max++ == 1) return;
                var indexCoords = CoordinateUtils.TranslateVector(rowOrigin, index, CoordinateUtils.Direction2E);
                var coordsSE = CoordinateUtils.TranslateVector(indexCoords, 1, CoordinateUtils.Direction2SE);
                var coordsSW = CoordinateUtils.TranslateVector(indexCoords, 1, CoordinateUtils.Direction2SW);
                
                try
                {           
                    var faceCoords = new Vector3I(indexCoords.X - 1, indexCoords.Y, 1); // Right
                    var face = new Face(faceCoords, new Vertex[]
                    {
                        vertexDict[CoordinateUtils.Vector2IToVector4I(indexCoords)],
                        vertexDict[CoordinateUtils.Vector2IToVector4I(coordsSW)],
                        vertexDict[CoordinateUtils.Vector2IToVector4I(coordsSE)],
                    });
                    
                    
                    _faceDict[faceCoords] = face;
                    AddChild(face);
                }
                catch (Exception)
                {
                    // One of the vertices doesn't exist, no problem.
                }
            }
        }
        
        TileMapLayerTerrain.Instance.TileClick += (cellCubeCoords, tileMapMousePosition) =>
        {;
            Cell cell = Cells.Instance.GetCell(cellCubeCoords);
            Vector2 localCoords = cell.ToLocal(tileMapMousePosition);
            
            double angle = (-localCoords.Angle() + Math.Tau) % Math.Tau;
            
            Vector3I faceCoords = GetFaceFromCellVertex(cellCubeCoords, angle);
            
            EmitSignal(SignalName.FaceClick, GetFace(faceCoords));
        };
        
        FaceClick += (face) => {
            GD.Print($"TILE CLICK: faceCoords {face.GetCoords()}");
        };
    }
}
