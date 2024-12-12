using Godot;
using System;

public partial class Vertex : Node2D
{
	
	private int _height;
	private Node2D _label;
	private Vector4I _coords;
	
	public Vertex(): this(new Vector4I(0, 0, 0, 0), 0) {}
	
	public Vertex(Vector4I coords, int height)
	{
		_coords = coords;
		_height = height;
	}
	
	public override void _Ready()
	{
		this._label = LabelUtils.CreateLabel($"{this._height}");
		this.AddChild(this._label);
	}
	
	private void _UpdateLabel()
	{
		this.RemoveChild(this._label);
		this._label = LabelUtils.CreateLabel($"{this._height}");
		this.AddChild(this._label);
	}
	
	public int GetHeight()
	{
		return _height;
	}
	
	public void SetHeight(int height)
	{
		_height = height;
		this._UpdateLabel();
	}
}

public partial class Vertices : Node
{	
	private Godot.Collections.Dictionary<Vector4I, Vertex> vertexDict = new Godot.Collections.Dictionary<Vector4I, Vertex>();
	
	private Vector2 _GetCirclePoint(int segmentIndex)
	{
		var tileMapLayerTerrain = GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");
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
	
	private static Vector4I[] TILE_TO_VERTEX_INDEX_CONVERSION = new Vector4I[] {
		new Vector4I(0, 0, 0, 0), // NW
		new Vector4I(-1, 0, 1, 1), // W
		new Vector4I(0, 1, -1, 0), // SW
		new Vector4I(0, 0, 0, 1), // SE
		new Vector4I(1, 0, -1, 0), // E
		new Vector4I(0, -1, 1, 1), // NE
		new Vector4I(0, 0, 0, 2) // center, lame
	};
	
	public static Vector2I CubeToOddQ(Vector3I hex)
	{
		var q = hex.X;
		var r = hex.Y;
		
		var col = q;
		var row = r + (q - (q&1)) / 2;
		return new Vector2I(col, row);
	}

	public static Vector3I OddQToCube(Vector2I oddQ)
	{
		var col = oddQ.X;
		var row = oddQ.Y;
		
		var q = col;
		var r = row - (col - (col&1)) / 2;
		return new Vector3I(q, r, -q-r);
	}
	
	public static Vector4I TileToVertexCoord(Vector3I tile, int index)
	{
		var converter = Vertices.TILE_TO_VERTEX_INDEX_CONVERSION[index];
		var tile4 = new Vector4I(tile.X, tile.Y, tile.Z, 0);
		return tile4 + converter;
	}

	public override void _Ready()
	{
		var tileMapLayerTerrain = GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");
		tileMapLayerTerrain.VertexClick += (cell, index) => {
			var mapCoords = tileMapLayerTerrain.LocalToMap(cell.GetPosition());
			var coords = Vertices.OddQToCube(mapCoords);
			var vertexCoords = Vertices.TileToVertexCoord(coords, index);
			
			GD.Print("Clicked one vertex", vertexCoords);
			var vertex = vertexDict[vertexCoords];
			vertex.SetHeight(vertex.GetHeight() + 1);
		};
		
		var tilMapLayerTerrain = GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");

		var cellsNode = GetNode<Cells>("/root/Root2D/TerrainSystem/Cells");
		var cells = cellsNode.GetCells();
		
		// Emergency! Break in case of bugs
		var i = 0;
		var m = 5;
		foreach (Node2D cell in cells) {
			if (++i > m) break;
			var mapCoords = tilMapLayerTerrain.LocalToMap(cell.GetPosition());
			var coords = Vertices.OddQToCube(mapCoords);
			
			var vertices = new Node2D();
			vertices.Name = "vertices";
			
			var centerVertexCoords = (
				Vertices.TileToVertexCoord(coords, 0) +
				new Vector4I(0, 0, 0, 2)
			);
			var centerVertex = new Vertex(centerVertexCoords, 0);
			vertices.AddChild(centerVertex);
			vertexDict[centerVertexCoords] = centerVertex;
			
			for (var index = 0; index < 6; index++) {
				var vertexCoords = Vertices.TileToVertexCoord(coords, index);
				if (vertexDict.ContainsKey(vertexCoords))
				{
					continue;
				}
				
				var vertex = new Vertex(vertexCoords, 0);
				vertex.SetPosition(this._GetCirclePoint(index));
				vertices.AddChild(vertex);
				
				vertexDict[vertexCoords] = vertex;
			}
			
			cell.AddChild(vertices);
		}
	}
	
	public Vertex GetVertex(Vector4I coords)
	{
		var cells = GetNode<Cells>("/root/Root2D/TerrainSystem/Cells");
		return cells.GetNode<Vertex>($"{coords}");
	}
}
