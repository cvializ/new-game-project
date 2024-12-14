using System;
using Godot;

public partial class SingleHeightNode2D : Node2D
{
    private int height = 0;
    private Node2D label;

    public override void _Ready()
    {
        this.label = new MyLabel($"{this.height}");
        this.AddChild(this.label);
    }

    private void UpdateLabel()
    {
        this.RemoveChild(this.label);
        this.label = new MyLabel($"{this.height}");
        this.AddChild(this.label);
    }

    public int GetHeight()
    {
        return this.height;
    }

    public void SetHeight(int height)
    {
        this.height = height;
        this.UpdateLabel();
    }
}

public partial class Height : Node
{
    public override void _Ready()
    {
        var tileMapLayerTerrain = this.GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");
        tileMapLayerTerrain.CellClick += (cell, vertex) =>
        {
            GD.Print("Clicked one hex", cell);
            var height = cell.GetNode<SingleHeightNode2D>("height");
            height.SetHeight(height.GetHeight() + 1);
        };

        var cells = this.GetNode<Cells>("/root/Root2D/TerrainSystem/Cells").GetCells();
        foreach (Node2D cell in cells)
        {
            var height = new SingleHeightNode2D();
            height.Name = "height";
            cell.AddChild(height);

            height.SetHeight(1);
        }
    }
}
