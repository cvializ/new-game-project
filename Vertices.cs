using System;
using System.Collections.Generic;
using Godot;

public partial class Vertex : Node2D
{
    [Signal]
    public delegate void VertexClickEventHandler(Vertex vertex, int height);

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
    }

    public override void _Ready()
    {
        _label = new MyLabel($"");
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
        EmitSignal(SignalName.VertexClick, this, height);
        UpdateLabel();
    }

    public Vector4I GetCoords()
    {
        return _coords;
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
    
    public Vector2I _PixelToVertexAxialCoords(Vector2I pixelCoords)
    {
        int row = pixelCoords.Y;
        int column = pixelCoords.X;
        
        // Handle side sawtooth
        Vector2I evenRowDelta = new Vector2I(0, 1);
        Vector2I oddRowDelta = new Vector2I(-1, 1);
        int pairs = row / 2;
        
        // Handle top sawtooth
        Vector2I thirdOfFourColumnsDelta = row % 2 == 0 ? new Vector2I(-1, 1) : new Vector2I(0, 1);
        var adjustment = (column - 2) % 3 == 0 ? thirdOfFourColumnsDelta : new Vector2I(0, 0);
        var next = pairs * (oddRowDelta + evenRowDelta) + (row % 2 == 0 ? new Vector2I(0, 0) : oddRowDelta);
        
        return next + new Vector2I(1, 0) * pixelCoords + adjustment;
    }

    //public Vector2 _VertexAxialCoordsToTriangleCoords(Vector2I coords)
    //{
        //Vector2 E = new Vector2(1, 0);
        //Vector2 SE = E.Rotated((float)(Math.PI / 3));
        //
        //return (E * coords.X + SE * coords.Y);
    //}

    // Useful once 0,1 is -1, 1
    public Vector2 _VertexCubeCoordsToGlobalCoords(Vector4I vertexCubeCoords)
    {
        Vector2 E = new Vector2(1, 0);
        Vector2 SW = E.Rotated((float)(2 * Math.PI / 3));
        Vector2 NW = E.Rotated((float)(-2 * Math.PI / 3));
        Vector2 SE = E.Rotated((float)(Math.PI / 3));
        
        int q = vertexCubeCoords.X;
        int r = vertexCubeCoords.Y;
        int s = vertexCubeCoords.Z;
        int w = vertexCubeCoords.W;
        
        return (q * E + r * SW + s * NW + w * SE) * 32;
    }


    public override void _Ready()
    {
        var tileMapLayerTerrain = this.GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");
        tileMapLayerTerrain.CellClick += (cell, index) =>
        {
            var mapCoords = tileMapLayerTerrain.LocalToMap(cell.GetPosition());
            var coords = MathUtils.OddQToCube(mapCoords);
            var vertexCoords = Vertices.TileToVertexCoord(coords, index);

            GD.Print("Cell: ", ((Cell)cell).GetCoords());
            var vertex = this.vertexDict[vertexCoords];
            vertex.SetHeight(vertex.GetHeight() + 1);
        };

        var tilMapLayerTerrain = this.GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");

        var cellsNode = this.GetNode<Cells>("/root/Root2D/TerrainSystem/Cells");
        var cells = cellsNode.GetCells();
        var heightMap = new HeightMap("./heightmap_sm.png");

        var size = heightMap.GetSize();
        
        Vector2 qPositionVector = new Vector2(1, 0) * 32;
        Vector2 rPositionVector = new Vector2(1, 0).Rotated((float)(Math.PI / 3)) * 32;

        Vector2 origin = qPositionVector / 2;
        
        var max = 0;
        for (int i = 0; i < size.X; i++)
        {
            //if (max++ == 10) return;
            for (int j = 0; j < size.Y; j++)
            {
                Vector2I pixelCoords = new Vector2I(i, j);
                Vector2I vertexAxialCoords = _PixelToVertexAxialCoords(pixelCoords);
                Vector4I vertexCubeCoords = CoordinateUtils.Vector2IToVector4I(vertexAxialCoords);              
                
                Vertex vertex = new Vertex(vertexCubeCoords, heightMap.GetHeightAtPixel(pixelCoords));
                Vector2 vertexGlobalCoords = _VertexCubeCoordsToGlobalCoords(vertexCubeCoords);
                vertex.SetGlobalPosition(origin + vertexGlobalCoords);
                
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
