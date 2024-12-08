using Godot;
using System;

public partial class TileMapLayer : Godot.TileMapLayer
{	
	private Node2D _CreateLabel(String text)
	{
		var container = new Node2D();
		var label = new Label();
		var theme = (Theme)ResourceLoader.Load("res://new_theme.tres", "Theme");
		var size = theme.DefaultFont.GetStringSize(text);
		
		label.SetTheme(theme);
		label.SetText(text);
		label.SetPosition(new Vector2(-size[0] / 2, -size[1] / 2));
		label.SetZIndex(1);
				
		container.AddChild(label);
		
		return container;
	}
	//
	//private void _AddLabelAtCoords(String text, Vector2[] coords)
	//{
		//var label = this._CreateLabel(text);
		//label.SetPosition(coords);
		//return label;
	//}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var tileSize = this.GetTileSet().GetTileSize();
		var usedCells = this.GetUsedCells();
		for (int i = 0; i < usedCells.Count; i++) 
		{			
			var cellCoordsLocal = this.MapToLocal(usedCells[i]);
			
			var centerLabel = this._CreateLabel($"{i}");
			centerLabel.SetPosition(cellCoordsLocal);
			this.AddChild(centerLabel);
			
			for (int j = 0; j < 6; j++) {
				var label = this._CreateLabel($"{j + 1}");
				
				var angle = 2 * Math.PI / 6 * j;
				
				var adjacent = tileSize[0] / 2 * Math.Cos(-angle);
				var opposite = tileSize[1] / 2 * Math.Sin(-angle);
				
				label.SetPosition(new Vector2(cellCoordsLocal.X + (float)adjacent, cellCoordsLocal.Y + (float)opposite));
				this.AddChild(label);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
