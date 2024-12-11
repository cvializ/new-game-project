using Godot;
using System;

public partial class HexTerrainTriangle : Node2D
{
	private int _index = 0;
	private Node2D _label;
	
	public override void _Ready()
	{
		this._label = LabelUtils.CreateLabel("T");
		this.AddChild(this._label);
	}
	
	private void _UpdateLabel()
	{
		this.RemoveChild(this._label);
		this._label = LabelUtils.CreateLabel("T");
		this.AddChild(this._label);
	}
	
	//public int GetHeight()
	//{
		//return _height;
	//}
	//
	//public void SetHeight(int height)
	//{
		//_height = height;
		//this._UpdateLabel();
	//}
}

public partial class Triangles : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
