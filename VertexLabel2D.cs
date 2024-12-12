using Godot;
using System;

public partial class HexLabel : Node2D
{
    private Vector2 _tileSize;

    public HexLabel() : this(new Vector2(0, 0)) { }

    public HexLabel(Vector2 _tileSize)
    {
        this._tileSize = _tileSize;
    }

    public override void _Ready()
    {
        //var centerLabel = LabelUtils.CreateLabel("0");
        //this.AddChild(centerLabel);
        //
        //for (int j = 0; j < 6; j++) {
        //var label = LabelUtils.CreateLabel($"{j + 1}");
        //
        //var angle = 2 * Math.PI / 6 * j;
        //
        //var adjacent = (this._tileSize[0] / 2.5) * Math.Cos(-angle);
        //var opposite = (this._tileSize[1] / 2.5) * Math.Sin(-angle);
        //
        //label.SetPosition(new Vector2((float)adjacent, (float)opposite));
        //this.AddChild(label);
        //}
    }
}

[GlobalClass]
public partial class VertexLabel2D : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Alive 2");
        var tileMapLayerTerrain = GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");
        var tileSize = tileMapLayerTerrain.GetTileSize();

        var cells = GetNode<Cells>("/root/Root2D/TerrainSystem/Cells").GetCells();
        foreach (Node2D cell in cells)
        {
            cell.AddChild(new HexLabel(tileSize));
        };
    }
}
