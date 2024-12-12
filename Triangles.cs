using Godot;
using System;
using System.Collections.Generic;

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
	private Vector2 _GetCirclePoint(int segmentIndex)
	{
		var angle = Math.PI / 3 * segmentIndex;
		var adjacent = (64 / 2) * Math.Cos(2 * Math.PI - angle);
		var opposite = (64 / 2) * Math.Sin(2 * Math.PI - angle);
		// Clamp
		if (opposite < -27)
		{
			opposite = -27;
		}
		if (opposite > 27)
		{
			opposite = 27;
		}
		return new Vector2((float)adjacent, (float)opposite);
	}

	public override void _Ready()
	{
		var triangles = new Node2D();

		for (int i = 0; i < 6; i++)
		{

		}
		List<Vector2> polygonPoints = new List<Vector2>();

		polygonPoints.Add(new Vector2(0, 0));
		polygonPoints.Add(this._GetCirclePoint(0));
		polygonPoints.Add(this._GetCirclePoint(1));

		var pointsArray = polygonPoints.ToArray();

		var polygon = new Polygon2D();
		polygon.SetPolygon(pointsArray);
		polygon.SetColor(new Color(0, 0, 0, .5f));

		this.AddChild(polygon);

		List<Vector2> polygonPoints2 = new List<Vector2>();

		polygonPoints2.Add(new Vector2(0, 0));
		polygonPoints2.Add(this._GetCirclePoint(1));
		polygonPoints2.Add(this._GetCirclePoint(2));

		var pointsArray2 = polygonPoints2.ToArray();

		var polygon2 = new Polygon2D();
		polygon2.SetPolygon(pointsArray2);
		polygon2.SetColor(new Color(0, 0, 0, .5f));

		this.AddChild(polygon2);

		List<Vector2> polygonPoints3 = new List<Vector2>();

		polygonPoints3.Add(new Vector2(0, 0));
		polygonPoints3.Add(this._GetCirclePoint(2));
		polygonPoints3.Add(this._GetCirclePoint(3));

		var pointsArray3 = polygonPoints3.ToArray();

		var polygon3 = new Polygon2D();
		polygon3.SetPolygon(pointsArray3);
		polygon3.SetColor(new Color(0, 0, 0, .5f));

		this.AddChild(polygon3);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
