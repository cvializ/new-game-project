using System;
using System.Linq;
using Godot;

public partial class Cell : Node2D
{
    [Signal]
    public delegate void CellClickEventHandler();

    private Vector3I _coords;

    public Cell() : this(new Vector3I(0, 0, 0))
    {
    }
    
    public Cell(Vector3I coords)
    {
        _coords = coords;
        this.Name = $"{coords}";
    }
    
    public Vector3I GetCoords()
    {
        return _coords;
    }
}

public partial class Cells : Node
{
    private Godot.Collections.Dictionary<Vector3I, Cell> _cellDict = new Godot.Collections.Dictionary<Vector3I, Cell>();

    public override void _Ready()
    {
        var tileMapLayerTerrain = this.GetNode<TileMapLayerTerrain>("/root/Root2D/TerrainSystem/TileMapLayerTerrain");
        var tileSize = tileMapLayerTerrain.GetTileSize();
        var usedCells = tileMapLayerTerrain.GetUsedCells();

        for (int i = 0; i < usedCells.Count; i++)
        {
            var mapCoords = usedCells[i];
            var cubeCoords = MathUtils.OddQToCube(mapCoords);
            
            var cell = new Cell(cubeCoords);
            _cellDict[cubeCoords] = cell;
            
            var cellCoordsLocal = tileMapLayerTerrain.MapToLocal(mapCoords);
            cell.SetPosition(cellCoordsLocal);
            
            this.AddChild(cell);
        }
    }

    public Godot.Collections.Array<Cell> GetCells()
    {
        return new Godot.Collections.Array<Cell>(_cellDict.Values);
    }

    public Cell GetCell(Vector3I cubeCoords)
    {
        return _cellDict[cubeCoords];
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
