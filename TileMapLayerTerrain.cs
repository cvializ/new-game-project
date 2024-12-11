using Godot;
using System;
using System.Linq;

public partial class TileMapLayerTerrain : TileMapLayer
{
  	[Signal]
  	public delegate void VertexClickEventHandler(Node2D cellDigits, int vertex);

	private double _NormalizedAtan2(double y, double x)
	{
		var angle = -Math.Atan2(y, x);
		return (angle > 0 ? angle : angle + Math.PI * 2);
	}
	
	public override void _Input(InputEvent @event)
	{
		if (!(@event is InputEventMouseButton mouseEvent))
		{
			return;
		}
		
		if (!mouseEvent.Pressed) {
			return;
		}
		
		var globalCoords = mouseEvent.Position;
		var localCoords = this.ToLocal(globalCoords);
		var mapCoords = this.LocalToMap(localCoords);
		
		// Not circular because this is called async
		var cells = GetNode<Cells>("/root/Root2D/TerrainSystem/Cells");
		var cell = cells.GetCell(mapCoords);
		var cellLocalCoords = cell.ToLocal(globalCoords);
		
		var length = Math.Sqrt(cellLocalCoords.X * cellLocalCoords.X + cellLocalCoords.Y * cellLocalCoords.Y);
		
		if (length < 10f) {
			EmitSignal(SignalName.VertexClick, cell, 0);
			return;
		}
		
		var angle = this._NormalizedAtan2(cellLocalCoords[1], cellLocalCoords[0]);
		//var vertex = Math.Floor((angle / (Math.PI / 3)) + Math.PI / 6) % 6;
		var rotated = (angle + 4 * Math.PI / 3) % (2 * Math.PI);
		var vertex = Math.Round(rotated / (Math.PI / 3));
		
		GD.Print($"vertex click {vertex}");
		
		EmitSignal(SignalName.VertexClick, cell, vertex);
	}
	
	public Vector2 GetTileSize()
	{
		return this.GetTileSet().GetTileSize();
	}
	
	public override void _Ready()
	{		
	}
}
