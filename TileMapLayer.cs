using Godot;
using System;


public partial class CircleNode2D : Godot.Node2D
{
	public override void _Draw()
	{
		this.DrawCircle(new Vector2(0f, 0f), 3f, new Color(1, 1, 1, 1));
	}
}

public partial class TileMapLayer : Godot.TileMapLayer
{	
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
				
		var digits = this._GetCellDigitsNode(mapCoords);
		var digitsLocalCoords = digits.ToLocal(globalCoords);
		
		var length = Math.Sqrt(digitsLocalCoords.X * digitsLocalCoords.X + digitsLocalCoords.Y * digitsLocalCoords.Y);
		
		if (length < 10f) {
			GD.Print("vertex", 0);
			return;
		}
		
		var angle = this._NormalizedAtan2(digitsLocalCoords[1], digitsLocalCoords[0]);
		var vertex = Math.Floor((angle / (Math.PI / 3)) + Math.PI / 6) % 6 + 1;
		
		GD.Print("vertex", vertex);
	}
	
	private Node2D _CreateLabel(String text)
	{
		var container = new Node2D();
		var label = new Label();
		var theme = (Theme)ResourceLoader.Load("res://new_theme.tres", "Theme");
		var size = theme.DefaultFont.GetStringSize(text);
		
		label.SetTheme(theme);
		label.SetText(text);
		
		label.SetPosition(new Vector2(-3, -7));
		label.SetZIndex(1);
		
		container.AddChild(label);
		
		return container;
	}
	
	private Node2D _GetCellDigitsNode(Vector2 mapCoords)
	{
		var cells = GetNode("cells");
		return (Node2D)cells.GetNode($"{mapCoords}");
	}
	
	public override void _Ready()
	{
		var cells = new Node2D();
		cells.Name = "cells";
		
		var tileSize = this.GetTileSet().GetTileSize();
		var usedCells = this.GetUsedCells();
		
		for (int i = 0; i < usedCells.Count; i++) 
		{			
			var digits = new CircleNode2D();
			
			var mapCoords = usedCells[i];
			var cellCoordsLocal = this.MapToLocal(mapCoords);
			
			digits.Name = $"{mapCoords}";
			digits.SetPosition(cellCoordsLocal);
			
			var centerLabel = this._CreateLabel($"{0}");
			digits.AddChild(centerLabel);
			
			for (int j = 0; j < 6; j++) {
				var label = this._CreateLabel($"{j + 1}");
				
				var angle = 2 * Math.PI / 6 * j;
				
				var adjacent = (tileSize[0] / 2.5) * Math.Cos(-angle);
				var opposite = (tileSize[1] / 2.5) * Math.Sin(-angle);
				
				label.SetPosition(new Vector2((float)adjacent, (float)opposite));
				digits.AddChild(label);
			}
			
			cells.AddChild(digits);
		}
		
		this.AddChild(cells);
		GD.Print(cells.Position);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
