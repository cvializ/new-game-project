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
        
        _label = LabelUtils.CreateLabel($"{_coords.W}");
        AddChild(_label);
    }

    private void UpdateLabel()
    {
        RemoveChild(_label);
        _label = LabelUtils.CreateLabel($"{_height}");
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

    private static Vector4I[] TileToVertexIndexConversion = new Vector4I[]
    {
        new Vector4I(0, 0, 0, 0), // NW
        new Vector4I(-1, 0, 1, 1), // W
        new Vector4I(0, 1, -1, 0), // SW
        new Vector4I(0, 0, 0, 1), // SE
        new Vector4I(1, 0, -1, 0), // E
        new Vector4I(0, -1, 1, 1), // NE
        new Vector4I(0, 0, 0, 2), // center, lame
    };

    public static Vector4I GetVertexFromCenter(Vector4I centerVertex, int index)
    {
        var nwVertex = centerVertex - new Vector4I(0, 0, 0, 2);
        return nwVertex + TileToVertexIndexConversion[index];
    }

    public static Vector4I TileToVertexCoord(Vector3I tile, int index)
    {
        var converter = Vertices.TileToVertexIndexConversion[index];
        var tile4 = new Vector4I(tile.X, tile.Y, tile.Z, 0);
        return tile4 + converter;
    }
    
    public List<Vertex> GetCenterVertices()
    {
        var centerVertices = new List<Vertex>();
        foreach (Vector4I key in this.vertexDict.Keys)
        {
            if (vertexDict[key].GetCoords().W == 2)
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

    public override void _Ready()
    {
        var tileMapLayerTerrain = this.GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");
        tileMapLayerTerrain.CellClick += (cell, index) =>
        {
            var mapCoords = tileMapLayerTerrain.LocalToMap(cell.GetPosition());
            var coords = MathUtils.OddQToCube(mapCoords);
            var vertexCoords = Vertices.TileToVertexCoord(coords, index);

            var vertex = this.vertexDict[vertexCoords];
            vertex.SetHeight(vertex.GetHeight() + 1);
        };

        var tilMapLayerTerrain = this.GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");

        var cellsNode = this.GetNode<Cells>("/root/Root2D/TerrainSystem/Cells");
        var cells = cellsNode.GetCells();

        foreach (Node2D cell in cells)
        {
            var mapCoords = tilMapLayerTerrain.LocalToMap(cell.GetPosition());
            var coords = MathUtils.OddQToCube(mapCoords);
            
            var cellGlobalPos = cell.GetGlobalPosition();

            for (var index = 0; index < 6; index++)
            {
                var vertexCoords = Vertices.TileToVertexCoord(coords, index);
                if (this.vertexDict.ContainsKey(vertexCoords))
                {
                    continue;
                }

                var vertex = new Vertex(vertexCoords, 0);
                vertex.VertexClick += (vertex, height) =>
                {
                    GD.Print("vertex click", vertex.GetCoords(), height);
                };

                var vertexCircleOffset = this.GetCirclePoint(index);
                var vertexGlobalPos = cellGlobalPos + vertexCircleOffset;
                vertex.SetGlobalPosition(vertexGlobalPos);
                
                this.AddChild(vertex);

                this.vertexDict[vertexCoords] = vertex;
            }

            var centerVertexCoords =
                Vertices.TileToVertexCoord(coords, 0) +
                new Vector4I(0, 0, 0, 2);
            var centerVertex = new Vertex(centerVertexCoords, 0);
            var centerVertexGlobalPos = cellGlobalPos;
            centerVertex.SetPosition(cell.GetPosition());
            
                centerVertex.VertexClick += (vertex, height) =>
                {
                    GD.Print("vertex click", vertex.GetCoords(), height);
                };
            this.AddChild(centerVertex);
            this.vertexDict[centerVertexCoords] = centerVertex;
        }
    }

    public Godot.Collections.Dictionary<Vector4I, Vertex> GetVertexDict()
    {
        return this.vertexDict;
    }
}
