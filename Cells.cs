using Godot;
using System;

public partial class Cells : Node
{
	public override void _Ready()
	{
		var tileMapLayerTerrain = GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");
		var tileSize = tileMapLayerTerrain.GetTileSize();
		var usedCells = tileMapLayerTerrain.GetUsedCells();
		
		for (int i = 0; i < usedCells.Count; i++) 
		{
			var mapCoords = usedCells[i];
			var cellCoordsLocal = tileMapLayerTerrain.MapToLocal(mapCoords);
			
			var cell = new Node2D();
			cell.Name = $"{mapCoords}";
			cell.SetPosition(cellCoordsLocal);
			
			this.AddChild(cell);
		}
	}

	
	public Godot.Collections.Array<Node> GetCells()
	{
		return this.GetChildren();

		// Saving for the short term:
		//var enumerable = (System.Collections.Generic.IEnumerable<Node>)children;
		//var array = enumerable.Select(x => (SingleHeightNode2D)x).ToArray();
		//return new Godot.Collections.Array<SingleHeightNode2D>(array);
	}
	
	public Node2D GetCell(Vector2 mapCoords)
	{
		return GetNode<Node2D>($"{mapCoords}");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
