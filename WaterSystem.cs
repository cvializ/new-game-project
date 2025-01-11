using Godot;
using System;
using System.Linq;

public partial class WaterUnit : Node
{
    private Face _face;
    
}

public partial class WaterSystem : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Cells.Instance.CellClick += (cell, index) =>
        {
            var oddQ = MathUtils.CubeToOddQ(cell.GetCoords());
            GD.Print("ODDQ", oddQ);
            
            var face = Faces.Instance.GetFace(new Vector3I(oddQ.X, oddQ.Y, 0));
            //face.Print();
            face.SetWater(true);
        };
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
