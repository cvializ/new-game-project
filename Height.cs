using Godot;
using System;

public partial class SingleHeightNode2D : Node2D
{
	private int _height = 0;
	private Node2D _label;
	
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

public partial class Height : Node
{
	public override void _Ready()
	{
		var tileMapLayerTerrain = GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");
		tileMapLayerTerrain.VertexClick += (cell, vertex) => {
			GD.Print("Clicked one hex", cell);
			var height = cell.GetNode<SingleHeightNode2D>("height");
			height.SetHeight(height.GetHeight() + 1);
		};
		
		var cells = GetNode<Cells>("/root/Root2D/TerrainSystem/Cells").GetCells();
		foreach (Node2D cell in cells) {
			var height = new SingleHeightNode2D();
			height.Name = "height";
			cell.AddChild(height);
			
			height.SetHeight(1);
		}
	}
}
