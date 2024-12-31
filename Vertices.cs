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
    public delegate void VertexClickEventHandler(Vertex vertex, int height);
    
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
    
    public void Print()
    {
        GD.Print(GetCoords(), GetHeight());
    }
}

public partial class Vertices : Node
{
    private Godot.Collections.Dictionary<Vector4I, Vertex> vertexDict = new Godot.Collections.Dictionary<Vector4I, Vertex>();
    private int _maxHeight = 15;

    private Vector2 GetCirclePoint(int segmentIndex)
    {
        var tileMapLayerTerrain = this.GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");
        var tileSize = tileMapLayerTerrain.GetTileSize();
        var radius = Math.Max(tileSize[0], tileSize[1]) / 2;
        var fudge = 2 * Math.PI / 3;
        var angle = fudge + (2 * Math.PI / 6 * segmentIndex);

        // Use -angle to adjust for Godot's coordinate being different
        // than my mental ones
        var adjacent = radius * Math.Cos(-angle);
        var opposite = radius * Math.Sin(-angle);
        return new Vector2((float)adjacent, (float)opposite);
    }
    
    public Vertex[] GetSiblings(Vertex vertex)
    {
        Vector4I vertexCoords = vertex.GetCoords();
        
        return new Vertex[] {
            vertexDict[CoordinateUtils.TranslateVector(vertexCoords, 1, CoordinateUtils.Direction4NW)],
            vertexDict[CoordinateUtils.TranslateVector(vertexCoords, 1, CoordinateUtils.Direction4W)],
            vertexDict[CoordinateUtils.TranslateVector(vertexCoords, 1, CoordinateUtils.Direction4SW)],
            vertexDict[CoordinateUtils.TranslateVector(vertexCoords, 1, CoordinateUtils.Direction4SE)],
            vertexDict[CoordinateUtils.TranslateVector(vertexCoords, 1, CoordinateUtils.Direction4E)],
            vertexDict[CoordinateUtils.TranslateVector(vertexCoords, 1, CoordinateUtils.Direction4NE)],
        };
    }
    
    public Vertex[] GetDownhillSiblings(Vertex vertex)
    {
        return GetSiblings(vertex).Where(sibling => sibling.GetHeight() > vertex.GetHeight()).ToArray();
    }

    public static Vector4I GetVertexFromCenter(Vector4I centerVertex, int index)
    {
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
    
    public List<Vertex> GetCenterVertices()
    {
        var centerVertices = new List<Vertex>();
        foreach (Vector4I key in this.vertexDict.Keys)
        {
            if (vertexDict[key].GetCoords().W == 1)
            {
                centerVertices.Add(vertexDict[key]);
            }
        }
        
        return centerVertices;
    }
    
    public Vertex GetVertex(Vector4I coords)
    {
        return vertexDict[coords];
    }
    
    public Vector2 VertexCubeCoordsToGlobalCoords(Vector4I cubeCoords)
    {
        var tileMapLayerTerrain = this.GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");
        var angle = new Vector2(1, 0).Rotated((float)(Math.PI / 3)) * 32;
        
        Vector2I oddQ = MathUtils.CubeToOddQ(new Vector3I(cubeCoords.X, cubeCoords.Y, cubeCoords.Z));
        Vector2 localPosition = tileMapLayerTerrain.MapToLocal(oddQ) + angle * cubeCoords.W - angle;
        
        return tileMapLayerTerrain.ToGlobal(localPosition);
    }

    public override void _Ready()
    {
        var tileMapLayerTerrain = this.GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");
        tileMapLayerTerrain.CellClick += (cell, index) =>
        {
            if (Control.Instance.GetSelectedControl() != 1)
            {
                return;
            }

            var mapCoords = tileMapLayerTerrain.LocalToMap(cell.GetPosition());
            var coords = MathUtils.OddQToCube(mapCoords);
            var vertexCoords = Vertices.TileToVertexCoord(coords, index);

            GD.Print("Cell: ", ((Cell)cell).GetCoords());
            var vertex = this.vertexDict[vertexCoords];
            vertex.SetHeight(vertex.GetHeight() + 1);
        };

        var tilMapLayerTerrain = this.GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");

        var heightMap = new HeightMap("./heightmap_sm.png");

        Vector2I size = heightMap.GetSize();
        
        var max = 0;
        for (int i = 0; i < size.X; i++)
        {
            for (int j = 0; j < size.Y; j++)
            {
                //if (max++ == 10) return;
                Vector2I pixelCoords = new Vector2I(i, j);
                Vector2I vertexAxialCoords = CoordinateUtils.PixelToVertexAxialCoords(pixelCoords);
                Vector4I vertexCubeCoords = CoordinateUtils.Vector2IToVector4I(vertexAxialCoords);              
                
                Vertex vertex = new Vertex(vertexCubeCoords, heightMap.GetHeightAtPixel(pixelCoords));
                Vector2 vertexGlobalCoords = VertexCubeCoordsToGlobalCoords(vertexCubeCoords);
                vertex.SetGlobalPosition(vertexGlobalCoords);
                
                this.vertexDict[vertexCubeCoords] = vertex;
                this.AddChild(vertex);
            }
        }
    }

    public Godot.Collections.Dictionary<Vector4I, Vertex> GetVertexDict()
    {
        return this.vertexDict;
    }
}
