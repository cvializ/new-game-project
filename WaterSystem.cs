using Godot;
using System;
using System.Linq;

public partial class WaterUnit : Node
{
    private Face _currentFace;
    //private Vector2I _momentum;
    
    public WaterUnit(Face face)
    {
        _currentFace = face;
    }
    
    public void Flow()
    {
        var downhill = _currentFace.GetDownhillDirection();
        var neighborCoords = _currentFace.GetNeighbor(downhill);
        
        GD.Print("FLOW DIRECTION ", downhill);
        
        var nextFace = Faces.Instance.GetFace(neighborCoords);
        if (nextFace.GetHasWater())
        {
            return;
        }
        
        _currentFace.SetWater(false);
        nextFace.SetWater(true);
        
        _currentFace = nextFace;
    }
    
    private double cumulativeDelta = 0;
    public override void _Process(double delta)
    {
        cumulativeDelta += delta;
        if (cumulativeDelta > .25)
        {
            Flow();
            cumulativeDelta = 0;
        }
    }
}

public partial class WaterSystem : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Cells.Instance.CellClick += (cell, index) =>
        //{
            //var oddQ = MathUtils.CubeToOddQ(cell.GetCoords());
            //GD.Print("ODDQ", oddQ);
            //
            //var face = Faces.Instance.GetFace(new Vector3I(oddQ.X, oddQ.Y, 0));
            ////face.Print();
            //face.SetWater(true);
        //};
        
        TileMapLayerTerrain.Instance.TileClick += (cellCubeCoords, tileMapMousePosition) =>
        {;
            Cell cell = Cells.Instance.GetCell(cellCubeCoords);
            Vector2 localCoords = cell.ToLocal(tileMapMousePosition);
            
            double angle = (-localCoords.Angle() + Math.Tau) % Math.Tau;
            
            Vector3I faceCoords = Faces.Instance.GetFaceFromCellVertex(cellCubeCoords, angle);
            
            Face face = Faces.Instance.GetFace(faceCoords);
            
            WaterUnit unit = new WaterUnit(face);
            AddChild(unit);
            //face.SetWater(true);
        };
        
        // TODO: put a lot of water in one place with a control
        // TODO: allow multiple waters to be on a face at once (this is weird but might be fun)
        // TODO: ^ port water to this class instead of the faces.
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
//
//public record WaterMovement(WaterHaver destination, float fraction);
//
//public class WaterProperties
//{
    //public float Mass = 0;
    //public float FlowRate = 1;
    //public Vector2 Velocity = new Vector2(0, 0);
    //public float SedimentLoad = 0;
    //public float NutrientLoad = 0;
    //
    //public WaterProperties() {}
    //
    //public WaterProperties(float mass, Vector2 velocity)
    //{
        //Mass = mass;
        //Velocity = velocity;
    //}
    //
    //public WaterProperties MergeWater(WaterProperties water)
    //{
        //float mass = Mass + water.Mass;
        //Vector2 velocity = Velocity + water.Velocity;
        //WaterProperties wp = new WaterProperties(mass, velocity);
        //
        //return wp;
    //}
//}
//
//public interface WaterHaver
//{
    ////public void SetWater(WaterProperties water);
    //public void MergeWater(WaterProperties water);
    //public WaterHaver[] GetRecipients();
    //public bool IsDownhill(WaterHaver downhill);
    //public void Flow();
//}
//
//public class WaterService
//{
    //
//}
//
//
//public partial class Face : Node2D, WaterHaver
//{
    //private WaterProperties _water = new WaterProperties();
    //
    //public void MergeWater(WaterProperties water)
    //{
        //_water = _water.MergeWater(water);
    //}
    //
    //// Vertices send water to edges
    //public WaterMovement[] GetRecipients()
    //{
        //return Faces.Instance.GetNeighbors(this).Cast<WaterHaver>().ToArray();
    //}
//}
//
//public partial class Vertex : Node2D, WaterHaver
//{    
    //private WaterProperties _water = new WaterProperties();
    //
    //public void MergeWater(WaterProperties water)
    //{
        //_water = _water.MergeWater(water);
    //}
    //
    //// Vertices send water to edges
    //public WaterHaver[] GetRecipients()
    //{
        //
        //return Faces.Instance.GetNeighbors(this).Cast<WaterHaver>().ToArray();
    //}
    //
    //public void Flow()
    //{
        //var recipients = GetRecipients();
//
        //foreach (WaterHaver recipient in recipients)
        //{
            //WaterProperties wp = new WaterProperties();
            //wp.Mass = _water.Mass / 6;
            //
            //recipient.MergeWater(wp);
        //}
    //}
//}
