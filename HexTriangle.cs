using Godot;
using System;
using System.Collections.Generic;

public partial class HexTriangle : Node2D
{
	private Vector2 _GetCirclePoint(int segmentIndex)
	{
		var angle = 2 * Math.PI / 6 * segmentIndex;
		var adjacent = (64 / 2) * Math.Cos(-angle);
		var opposite = (55 / 2) * Math.Sin(-angle);
		return new Vector2((float)adjacent, (float)opposite);
	}
	
	public override void _Ready() 
	{
		List<Vector2> polygonPoints = new List<Vector2>();
		
		
		
		polygonPoints.Add(new Vector2(0, 0));
		polygonPoints.Add(this._GetCirclePoint(0));
		polygonPoints.Add(this._GetCirclePoint(1));
		
		//polygonPoints.Add(new Vector2(10, 0));
		//polygonPoints.Add(new Vector2(10, 10));
		//polygonPoints.Add(new Vector2(0, 10));
		//
				
		var pointsArray = polygonPoints.ToArray();
		
		var polygon = new Polygon2D();
		polygon.SetPolygon(pointsArray);
		polygon.SetColor(new Color(0, 0, 0, .5f));
		
		this.AddChild(polygon);
	}
}
