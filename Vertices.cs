using Godot;
using System;

public partial class VertexNode : Node2D
{

}

public partial class Vertices : Node
{
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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Vertices");
		//var vertices = new Node();
		//vertices.Name = "vertices";
		
		var tilMapLayerTerrain = GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");
		
		var cells = GetNode<Cells>("/root/Root2D/TerrainSystem/Cells").GetCells();
		foreach (Node2D cell in cells) {	
			var mapCoords = tilMapLayerTerrain.LocalToMap(cell.GetPosition());
			
			var coords = Vertices.OddQToCube(mapCoords);
			GD.Print("Coords: ", coords);
			
			// constant q = N/S
			// constant r = NW/SE
			// constant s = SW/NE
			
			//if (cell.GetNode($"{coords}") ) 
			//{
				//continue;
			//}
			
			var vertex = new VertexNode();
			vertex.Name = $"{coords}";
			
			var label = LabelUtils.CreateLabel($"{coords}");
			vertex.AddChild(label);
			
			cell.AddChild(vertex);
		}
		
		//this.AddChild(vertices);
	}
	
	public VertexNode GetVertex(Vector2 coord)
	{
		return GetNode<VertexNode>($"{coord}");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
