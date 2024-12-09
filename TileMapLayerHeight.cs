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

public partial class TileMapLayerHeight : TileMapLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var tileMapLayerTerrain = GetNode<TileMapLayerTerrain>("TileMapLayerTerrain");
		tileMapLayerTerrain.VertexClick += (cellDigits, vertex) => {
			GD.Print("Clicked one hex");
			var height = (SingleHeightNode2D)cellDigits.GetNode("height");
			height.SetHeight(height.GetHeight() + 1);
		};
		
		
		var cells = tileMapLayerTerrain.GetCells();
		foreach (Node2D cell in cells) {
			var height = new SingleHeightNode2D();
			height.Name = "height";
			height.SetHeight(1);
		
			cell.AddChild(height);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
