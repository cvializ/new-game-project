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

    //private static Vector4I[] TileToVertexIndexConversion = new Vector4I[]
    //{
        //new Vector4I(0, 0, 0, 0), // NW
        //new Vector4I(-1, 0, 1, 2), // W
        //new Vector4I(0, 1, -1, 0), // SW
        //new Vector4I(0, 0, 0, 2), // SE
        //new Vector4I(1, 0, -1, 0), // E
        //new Vector4I(0, -1, 1, 2), // NE
        //new Vector4I(0, 0, 0, 1), // center, lame
    //};

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
        
        return CoordinateUtils.TranslateVector(centerVertex, 1, direction);
    }

    public static Vector4I TileToVertexCoord(Vector3I tile, int index)
    {
        //var converter = Vertices.TileToVertexIndexConversion[index];
        var centerVertex = new Vector4I(tile.X, tile.Y, tile.Z, 1);
        return GetVertexFromCenter(centerVertex, index);
        //return tile4 + converter;
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
    
    public Vector2I _PixelToVertexAxialCoords(Vector2I pixelCoords)
    {
        Vector2 y0 = new Vector2(1, 0).Rotated((float)(2 * Math.PI / 3));//new Vector2I(-1, 1);
        Vector2 y1 = new Vector2(1, 0).Rotated((float)(Math.PI / 3));//new Vector2I(0, 1);
        Vector2[] yArray = {
            new Vector2(0, 0),
            y0,
        };
        int halfY = (int)Math.Floor((decimal)pixelCoords.Y);
        Vector2I yPathEven = (y0 + y1) * halfY;
        Vector2I conversionY = (y0 + y1) * halfY + yArray[pixelCoords.Y % 2];
        
        Vector2 x0 = new Vector2(1, 0);
        Vector2 x1 = new Vector2(0, 1);
        Vector2 x2 = new Vector2(1, 0);
        Vector2 x3 = new Vector2(1, -1);
        Vector2I[] xArray = {
            new Vector2I(0, 0),
            x0,
            x0 + x1,
            x0 + x1 + x2,
        };
        
        int fourthX = (int)Math.Floor((decimal)(pixelCoords.X / 4));
        
        Vector2I conversionX = (x0 + x1 + x2 + x3) * fourthX + xArray[pixelCoords.X % 4];
        
        GD.Print("initial", pixelCoords, "conversion", conversionX + conversionY);
        
        return pixelCoords + conversionX + conversionY;
    }

    public override void _Ready()
    {
        var tileMapLayerTerrain = this.GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");
        tileMapLayerTerrain.CellClick += (cell, index) =>
        {
            var mapCoords = tileMapLayerTerrain.LocalToMap(cell.GetPosition());
            var coords = MathUtils.OddQToCube(mapCoords);
            var vertexCoords = Vertices.TileToVertexCoord(coords, index);

            GD.Print(vertexCoords);
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
            for (int j = 0; j < size.Y; j++)
            {
                if (max++ == 5) return;
                Vector2I pixelCoords = new Vector2I(i, j);
                Vector2I vertexAxialCoords = _PixelToVertexAxialCoords(pixelCoords);
                Vector4I vertexCubeCoords = CoordinateUtils.Vector2IToVector4I(vertexAxialCoords);
                
                GD.Print($"vertexAxialCoords {vertexAxialCoords} vertexCubeCoords {vertexCubeCoords}");
                
                Vertex vertex = new Vertex(vertexCubeCoords, j);//heightMap.GetHeightAtPixel(pixelCoords));
                
                
                int q = vertexAxialCoords.X;
                int r = vertexAxialCoords.Y;
                Vector2 vertexGlobalPos = qPositionVector * q + rPositionVector * r;
                vertex.SetGlobalPosition(origin + vertexGlobalPos);
                
                this.vertexDict[vertexCubeCoords] = vertex;
                this.AddChild(vertex);
            }
        }
//
        //var max = 0;//
        //foreach (Node2D cell in cells)
        //{
            ////if (max++ > 6) break;
            //var mapCoords = tilMapLayerTerrain.LocalToMap(cell.GetPosition());
            //var coords = MathUtils.OddQToCube(mapCoords);
            //
            //var cellGlobalPos = cell.GetGlobalPosition();
//
            //for (var index = 0; index < 6; index++)
            //{
                //var vertexCoords = Vertices.TileToVertexCoord(coords, index);
                //if (this.vertexDict.ContainsKey(vertexCoords))
                //{
                    //continue;
                //}
 //
                //var cube = new Vector3I(vertexCoords.X, vertexCoords.Y, vertexCoords.Z);
                //var pixelVector = MathUtils.CubeToOddQ(cube); // this needs to consider center vertices?
                ////GD.Print($"coords {coords} pixelVector {pixelVector}");
                //Color pixel = heightMap.GetImage().GetPixelv(pixelVector);
                //var height = (int)Math.Round(pixel.Luminance * _maxHeight);
//
                //var vertex = new Vertex(vertexCoords, height);
                //var vertexCircleOffset = this.GetCirclePoint(index);
                //var vertexGlobalPos = cellGlobalPos + vertexCircleOffset;
                //vertex.SetGlobalPosition(vertexGlobalPos);
                //
                //this.AddChild(vertex);
//
                //this.vertexDict[vertexCoords] = vertex;
            //}
//
            //var centerVertexCoords =
                //Vertices.TileToVertexCoord(coords, 0) +
                //new Vector4I(0, 0, 0, 1);
            //
            //var pixelVectorC = MathUtils.CubeToOddQ(coords);
            //Color pixelC = heightMap.GetImage().GetPixelv(pixelVectorC);
            //var heightC = (int)Math.Round(pixelC.Luminance * _maxHeight);
//
            //var centerVertex = new Vertex(centerVertexCoords, heightC);
            //var centerVertexGlobalPos = cellGlobalPos;
            //centerVertex.SetPosition(cell.GetPosition());
//
            //this.AddChild(centerVertex);
            //this.vertexDict[centerVertexCoords] = centerVertex;
        //}
    }

    public Godot.Collections.Dictionary<Vector4I, Vertex> GetVertexDict()
    {
        return this.vertexDict;
    }
}
