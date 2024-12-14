using System;
using System.Collections.Generic;
using Godot;

public partial class Vertex : Node2D
{
    [Signal]
    public delegate void VertexClickEventHandler();

    private int height;
    private Node2D label;
    private Vector4I coords;

    public Vertex()
        : this(new Vector4I(0, 0, 0, 0), 0)
    {
    }

    public Vertex(Vector4I coords, int height)
    {
        this.coords = coords;
        this.height = height;
    }

    public override void _Ready()
    {
        
        this.label = LabelUtils.CreateLabel($"{this.coords.W}");
        this.AddChild(this.label);
    }

    private void UpdateLabel()
    {
        this.RemoveChild(this.label);
        this.label = LabelUtils.CreateLabel($"{this.height}");
        this.AddChild(this.label);
    }

    public int GetHeight()
    {
        return this.height;
    }

    public void SetHeight(int height)
    {
        this.height = height;
        this.EmitSignal(SignalName.VertexClick);
        this.UpdateLabel();
    }

    public Vector4I GetCoords()
    {
        return this.coords;
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

    // TODO: Add center vertex so tiles can have a height themselves? Optional
    // TODO: I need to start writing tests
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

    public static Vector2I CubeToOddQ(Vector3I hex)
    {
        var q = hex.X;
        var r = hex.Y;

        var col = q;
        var row = r + ((q - (q & 1)) / 2);
        return new Vector2I(col, row);
    }

    public static Vector3I OddQToCube(Vector2I oddQ)
    {
        var col = oddQ.X;
        var row = oddQ.Y;

        var q = col;
        var r = row - ((col - (col & 1)) / 2);
        return new Vector3I(q, r, -q - r);
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
            var coords = Vertices.OddQToCube(mapCoords);
            var vertexCoords = Vertices.TileToVertexCoord(coords, index);

            var vertex = this.vertexDict[vertexCoords];
            vertex.SetHeight(vertex.GetHeight() + 1);
        };

        var tilMapLayerTerrain = this.GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");

        var cellsNode = this.GetNode<Cells>("/root/Root2D/TerrainSystem/Cells");
        var cells = cellsNode.GetCells();

        // Emergency! Break in case of bugs
        //var i = 0;
        //var m = 5;
        foreach (Node2D cell in cells)
        {
            // if (++i > m) break;
            var mapCoords = tilMapLayerTerrain.LocalToMap(cell.GetPosition());
            var coords = Vertices.OddQToCube(mapCoords);
            
            var cellGlobalPos = cell.GetGlobalPosition();

            for (var index = 0; index < 6; index++)
            {
                var vertexCoords = Vertices.TileToVertexCoord(coords, index);
                if (this.vertexDict.ContainsKey(vertexCoords))
                {
                    continue;
                }

                var vertex = new Vertex(vertexCoords, 0);
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
            
            this.AddChild(centerVertex);
            this.vertexDict[centerVertexCoords] = centerVertex;
        }
    }

    public Godot.Collections.Dictionary<Vector4I, Vertex> GetVertexDict()
    {
        return this.vertexDict;
    }
}
