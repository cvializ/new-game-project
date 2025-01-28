using System;
using System.Linq;
using System.Collections.Generic;
using Godot;

//
// Vertex data
//
// height
// edges
// faces
// rate of flow
// rate of evaporation
// rate of infiltration
// rate of 

public partial class Vertex : Node2D
{    
    [Signal]
    public delegate void OnHeightChangeEventHandler(Vertex vertex, int height);

    private int _height;
    private Node2D _label;
    private Vector4I _coords;

    public Vertex()
        : this(new Vector4I(0, 0, 0, 0), 0)
    {
    }

    public Vertex(Vector4I coords, int height)
    {
        _coords = coords;
        _height = height;
        
        ViewControl.Instance.OnChangeShowVertices += (showVertices) => {
            _label.SetVisible(showVertices);
        };
    }

    public override void _Ready()
    {
        //_label = new MyLabel($"V");
        _label = new MyLabel($"{_height}");
        AddChild(_label);
    }

    private void UpdateLabel()
    {
        RemoveChild(_label);
        _label = new MyLabel($"{_height}");
        AddChild(_label);
    }

    public int GetHeight()
    {
        return _height;
    }

    public void SetHeight(int height)
    {
        _height = height;
        EmitSignal(SignalName.OnHeightChange, this, height);
        UpdateLabel();
    }

    public Vector4I GetCoords()
    {
        return _coords;
    }
}

public partial class Vertices : Node
{
    [Signal]
    public delegate void VertexClickEventHandler(Vertex vertex, int height);
    
    public static Vertices Instance;
    public Vertices()
    {
        Instance = this;
    }
    
    private Godot.Collections.Dictionary<Vector4I, Vertex> _vertexDict = new Godot.Collections.Dictionary<Vector4I, Vertex>();
    private int _maxHeight = 15;
    
    public Edge[] GetEdges(Vertex vertex)
    {
        return GetSiblings(vertex).Select(siblingVertex => {
            return Edges.Instance.GetEdge(vertex, siblingVertex);
        }).ToArray();
    }
    
    public Vertex[] GetSiblings(Vertex vertex)
    {
        Vector4I vertexCoords = vertex.GetCoords();
        
        return new Vertex[] {
            _vertexDict[CoordinateUtils.TranslateVector(vertexCoords, 1, CoordinateUtils.Direction4NW)],
            _vertexDict[CoordinateUtils.TranslateVector(vertexCoords, 1, CoordinateUtils.Direction4W)],
            _vertexDict[CoordinateUtils.TranslateVector(vertexCoords, 1, CoordinateUtils.Direction4SW)],
            _vertexDict[CoordinateUtils.TranslateVector(vertexCoords, 1, CoordinateUtils.Direction4SE)],
            _vertexDict[CoordinateUtils.TranslateVector(vertexCoords, 1, CoordinateUtils.Direction4E)],
            _vertexDict[CoordinateUtils.TranslateVector(vertexCoords, 1, CoordinateUtils.Direction4NE)],
        };
    }
    
    public Vertex[] GetDownhillSiblings(Vertex vertex)
    {
        return GetSiblings(vertex).Where(sibling => sibling.GetHeight() > vertex.GetHeight()).ToArray();
    }

    public static Vector4I GetVertexFromCenter(Vector4I centerVertex, int index)
    {
        if (centerVertex.W != 1)
        {
            GD.Print("SUCKSSSS");
        }
        //var nwVertex = centerVertex - new Vector4I(0, 0, 0, 1);
        Vector4I direction;
        switch (index) 
        {
            case 0:
                direction = CoordinateUtils.Direction4NW;
                break;
            case 1:
                direction = CoordinateUtils.Direction4W;
                break;
            case 2:
                direction = CoordinateUtils.Direction4SW;
                break;
            case 3:
                direction = CoordinateUtils.Direction4SE;
                break;
            case 4:
                direction = CoordinateUtils.Direction4E;
                break;
            case 5:
                direction = CoordinateUtils.Direction4NE;
                break;
            case 6:
                direction = new Vector4I(0, 0, 0, 0);
                break;
            default:
                direction = new Vector4I(999, 999, 999, 999);
                break;
        }
        
        return centerVertex + direction;
    }

    public static Vector4I TileToVertexCoord(Vector3I tile, int index)
    {
        var centerVertex = new Vector4I(tile.X, tile.Y, tile.Z, 1);
        return GetVertexFromCenter(centerVertex, index);
    }
    
    public Vertex GetVertex(Vector4I vertexCoords)
    {
        try 
        {
            return _vertexDict[vertexCoords];
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    //public Vertex GetVertex(Vector2I axialCoords)
    //{
        //
        //
        //return GetVertex(new Vector4I());
    //}
    
    public Vector2 VertexCubeCoordsToGlobalCoords(Vector4I cubeCoords)
    {
        var angle = new Vector2(1, 0).Rotated((float)(Math.PI / 3)) * 32;
        
        Vector2I oddQ = MathUtils.CubeToOddQ(new Vector3I(cubeCoords.X, cubeCoords.Y, cubeCoords.Z));
        Vector2 localPosition = TileMapLayerTerrain.Instance.MapToLocal(oddQ) + angle * cubeCoords.W - angle;
        
        return TileMapLayerTerrain.Instance.ToGlobal(localPosition);
    }

    public override void _Ready()
    {
        Cells.Instance.CellClick += (cell, index) =>
        {
            if (Control.Instance.GetSelectedControl() != 1)
            {
                return;
            }

            var vertexCoords = Vertices.TileToVertexCoord(cell.GetCoords(), index);

            GD.Print("Cell: ", ((Cell)cell).GetCoords());
            var vertex = _vertexDict[vertexCoords];
            vertex.SetHeight(vertex.GetHeight() + 1);
            
            EmitSignal(SignalName.VertexClick, vertex);
        };

        var heightMap = TerrainHeightMap.Instance.GetHeightMap();

        Vector2I size = heightMap.GetSize();
        
        //var max = 0;
        for (int i = 0; i < size.X; i++)
        {
            //if (max++ == 2) return;
            for (int j = 0; j < size.Y; j++)
            {
                Vector2I pixelCoords = new Vector2I(i, j);
                Vector2I vertexAxialCoords = CoordinateUtils.PixelToVertexAxialCoords(pixelCoords);
                Vector4I vertexCubeCoords = CoordinateUtils.Vector2IToVector4I(vertexAxialCoords);              
                
                Vertex vertex = new Vertex(vertexCubeCoords, heightMap.GetHeightAtPixel(pixelCoords));
                Vector2 vertexGlobalCoords = VertexCubeCoordsToGlobalCoords(vertexCubeCoords);
                vertex.SetGlobalPosition(vertexGlobalCoords);
                
                _vertexDict[vertexCubeCoords] = vertex;
                AddChild(vertex);
            }
        }
    }

    public Godot.Collections.Dictionary<Vector4I, Vertex> GetVertexDict()
    {
        return _vertexDict;
    }
}
