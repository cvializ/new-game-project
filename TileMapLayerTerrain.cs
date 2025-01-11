using System;
using System.Linq;
using Godot;

public partial class TileMapLayerTerrain : TileMapLayer
{
    public static TileMapLayerTerrain Instance;
    public TileMapLayerTerrain()
    {
        Instance = this;
    } 
    
    [Signal]
    public delegate void CellClickEventHandler(Cell cell, int vertex);
    
    [Signal]
    public delegate void TileClickEventHandler(Vector3I cubeCoords, Vector2 localPosition);

    private Vector2 GetCirclePoint(int segmentIndex)
    {
        var angle = Math.PI / 3 * segmentIndex;
        var adjacent = (64 / 2) * Math.Cos((2 * Math.PI) - angle);
        var opposite = (64 / 2) * Math.Sin((2 * Math.PI) - angle);

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

    private double NormalizedAtan2(double y, double x)
    {
        var angle = -Math.Atan2(y, x);
        return angle > 0 ? angle : angle + (Math.PI * 2);
    }

    private void _EmitCellClickSignal(Cell cell, int vertex)
    {
        GD.Print("Click", cell.GetCoords());
        EmitSignal(SignalName.CellClick, cell, vertex);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!(@event is InputEventMouseButton mouseEvent))
        {
            return;
        }

        if (!mouseEvent.Pressed)
        {
            return;
        }
        
        var globalCoords = GetGlobalMousePosition();
        var localCoords = this.ToLocal(globalCoords);
        var mapCoords = this.LocalToMap(localCoords);
        // Not circular because this is called async
        var cells = this.GetNode<Cells>("/root/Root2D/TerrainSystem/Cells");
        var cubeCoords = MathUtils.OddQToCube(mapCoords);
        var cell = cells.GetCell(cubeCoords);
        
        var cellLocalCoords = cell.ToLocal(globalCoords);
        
        EmitSignal(SignalName.TileClick, cubeCoords, cellLocalCoords.Normalized());
        
        var length = cellLocalCoords.Length();

        if (length < 15f)
        {
            _EmitCellClickSignal(cell, 6);
            return;
        }

        var angle = -cellLocalCoords.Angle(); //this.NormalizedAtan2(cellLocalCoords[1], cellLocalCoords[0]);
        //GD.Print($"my angle {angle} vs vector angle ${cellLocalCoords.Angle()}");
        var rotated = (angle + (4 * Math.PI / 3)) % (2 * Math.PI);
        var vertex = (int)Math.Round(rotated / (Math.PI / 3));
                
       _EmitCellClickSignal(cell, vertex);
    }

    public Vector2I GetTileSize()
    {
        return this.GetTileSet().GetTileSize();
    }

    public override void _Ready()
    {
        HeightMap _heightMap = TerrainHeightMap.Instance.GetHeightMap();
        Vector2I size = _heightMap.GetSize();
        
        int cellsWide = (int)(size.X / 1.5) - 1;
        int cellsTall = size.Y / 2;
        
        GD.Print($"Size is {size}");
        GD.Print($"Map is {cellsWide}x{cellsTall}");
        
        var dirtTile = new Vector2I(0, 0);
        var grassTile = new Vector2I(1, 0);
        GD.Print($"SetCell {this.GetTileMapDataAsArray()}");
        
        for (int i = 0; i < cellsWide; i++)
        {
            for (int j = 0; j < cellsTall; j++)
            {
                SetCell(new Vector2I(i, j), 4, grassTile);
            }
        }
        
        CellClick += (cell, index) =>
        {
            if (Control.Instance.GetSelectedControl() != 0)
            {
                return;
            }

            SetCell(MathUtils.CubeToOddQ(cell.GetCoords()), 0, TileControl.Instance.GetSelectedTile());
        };
    }
}
