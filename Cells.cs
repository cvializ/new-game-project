using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Cell : Node2D
{
    private Vector3I _coords;

    public Cell() : this(new Vector3I(0, 0, 0))
    {
    }
    
    public Cell(Vector3I coords)
    {
        _coords = coords;
        Name = $"{coords}";
    }
    
    public Vector3I GetCoords()
    {
        return _coords;
    }
}

public partial class Cells : Node
{
    [Signal]
    public delegate void CellClickEventHandler(Cell cell, int vertex);
    
    public static Cells Instance;
    
    public Cells()
    {
        Instance = this;
    }
    
    private Godot.Collections.Dictionary<Vector3I, Cell> _cellDict = new Godot.Collections.Dictionary<Vector3I, Cell>();

    public Cell GetCell(Vector3I cellCubeCoords)
    {
        return _cellDict[cellCubeCoords];
    }
    
    public Face[] GetFaces(Vector3I cellCubeCoords)
    {
        Cell cell = _cellDict[cellCubeCoords];
        
        List<Face> faces = new List<Face>();
        for (double angle = 0; angle < 2 * Math.PI; angle += Math.PI / 3)
        {
            Vector3I faceCoords = Faces.Instance.GetFaceFromCellVertex(cell.GetCoords(), angle);
            Face face = Faces.Instance.GetFace(faceCoords);
            int index = (int)(angle / (Math.PI / 3));
            faces.Add(face);
        }
        
        return faces.ToArray();
    }
    //
    public int GetVertexIndexFromDirection(Vector2 direction)
    {
        double length = direction.Length();
        double angle = direction.Angle();
        double normalizedAngle = (-angle + Math.Tau) % Math.Tau;
        double rotatedAngle = (normalizedAngle + (4 * Math.PI / 3)) % (2 * Math.PI);
        var vertexIndex = length < 10f ? 6 : (int)Math.Round(rotatedAngle / (Math.PI / 3));
        
        return vertexIndex;
    }

    public Godot.Collections.Array<Cell> GetCells()
    {
        return new Godot.Collections.Array<Cell>(_cellDict.Values);
    }
    
    public override void _Ready()
    {
        var tileSize = TileMapLayerTerrain.Instance.GetTileSize();
        var usedCells = TileMapLayerTerrain.Instance.GetUsedCells();

        for (int i = 0; i < usedCells.Count; i++)
        {
            var mapCoords = usedCells[i];
            var cubeCoords = MathUtils.OddQToCube(mapCoords);
            
            var cell = new Cell(cubeCoords);
            _cellDict[cubeCoords] = cell;
            
            var cellCoordsLocal = TileMapLayerTerrain.Instance.MapToLocal(mapCoords);
            cell.SetPosition(cellCoordsLocal);
            
            AddChild(cell);
        }
        
        TileMapLayerTerrain.Instance.TileClick += (tileCoords, tileMapMousePosition) => 
        {
            Cell cell = GetCell(tileCoords);
            
            Vector2 cellLocalCoords = cell.ToLocal(tileMapMousePosition);
            int vertexIndex = GetVertexIndexFromDirection(cellLocalCoords);
            
            GD.Print($"CELL CLICK tileCoords {tileCoords} vertexIndex {vertexIndex}");
            
            EmitSignal(SignalName.CellClick, GetCell(tileCoords), vertexIndex);
        };
         
        CellClick += (cell, index) =>
        {
            if (Control.Instance.GetSelectedControl() != 0)
            {
                return;
            }

            TileMapLayerTerrain.Instance.SetCell(MathUtils.CubeToOddQ(cell.GetCoords()), 0, TileControl.Instance.GetSelectedTile());
        };
        
    }
}
