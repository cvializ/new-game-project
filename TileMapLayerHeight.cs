using Godot;
using System;

public partial class TileMapLayerHeight : TileMapLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var tileMapLayerTerrain = GetNode<TileMapLayerTerrain>("TileMapLayerTerrain");
		tileMapLayerTerrain.	VertexClick += (cellDigits, vertex) => GD.Print("VertexClickAngles", vertex);
		
		GD.Print(tileMapLayerTerrain.GetUsedCells());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
