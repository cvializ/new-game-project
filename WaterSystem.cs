using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class WaterUnit : Node
{
    public static System.Random Random = new System.Random();

    private float _rate = 1;
    private float _amount = 0;
    private Face _currentFace;
    //private Vector2 _momentum;
    
    public WaterUnit(Face face) : this(face, 1f) {}
    
    public WaterUnit(Face face, float amount)
    {
        _currentFace = face;
        _amount = amount;
        //_momentum = momentum;
        
        face.SetWater(true);
    }
    
    public Vector2 GetRandomDirection()
    {
        float randomAngle = (float)(Math.PI * 2 * WaterUnit.Random.NextDouble());
        return Vector2.Right.Rotated(randomAngle).Normalized();
    }
    
    private static Vector2 AxisNS =  Vector2.Up;
    private static Vector2 AxisSwNe = Vector2.Right.Rotated((float)(2 * Math.PI / 3));
    private static Vector2 AxisSeNw = Vector2.Right.Rotated((float)(Math.PI / 3));
    
    private Vector2[] Axes = new Vector2[] { AxisNS, AxisSwNe, AxisSeNw };
    
    public void Flow()
    {
        // N, SW, SE or S, NE, NW. Same axes whether left or right
        List<Vector3I> waterDestinations = Faces.GetNeighbors(_currentFace.GetCoords());
        
        bool isLeft = Faces.IsLeftFace(_currentFace.GetCoords());
        
        Vector2 downhillDirection = _currentFace.GetDownhillDirection();
        GD.Print($"downhill {downhillDirection}");
        
        List<float> coefficients = Axes
            //.Select(axis => axis.Dot(downhillDirection)).ToList();
            .Select(axis => (axis * (isLeft ? 1 : -1)).Dot(downhillDirection)).ToList();
        
        for (int i = 0; i < coefficients.Count; i++)
        {
            float coefficient = coefficients[i];
            
            GD.Print($"coeff {coefficient}");
            
            if (Math.Abs(coefficient) > float.Epsilon && coefficient < 0)
            {
                continue;
            }

            Face face = Faces.Instance.GetFace(waterDestinations[i]);
            face.SetWater(true);
            
            GD.Print($"next: ${waterDestinations[i]}");
            WaterSystem.Instance.GetOrCreateWaterUnit(waterDestinations[i]);
        }
        
        //var downhill = _currentFace.GetDownhillDirection() + _momentum;
        //var downhill = _currentFace.GetDownhillDirection();
        //
        //if (downhill == new Vector2(0, 0))
        //{
            //downhill = _currentFace.GetRandomDirection();
        //}
        //
        //var neighborCoords = _currentFace.GetNeighbor(downhill);
        //
        ////GD.Print("FLOW DIRECTION ", downhill);
        //
        //var nextFace = Faces.Instance.GetFace(neighborCoords);
        //if (nextFace.GetHasWater())
        //{
            //downhill = _currentFace.GetRandomDirection();
            ////return;
        //}
        //
        //_momentum += nextFace.GetDownhillDirection();
        ////_momentum = new Vector2(0, 0);
        //
        //_currentFace.SetWater(false);
        //nextFace.SetWater(true);
        //
        //_currentFace = nextFace;
    }
    
    //public override void _Ready()
    //{
        //Flow();
    //}
    
    private double cumulativeDelta = 0;
    public override void _Process(double delta)
    {
        cumulativeDelta += delta;
        if (cumulativeDelta > .1)
        {
            Flow();
            cumulativeDelta = 0;
        }
    }
}

public partial class WaterSystem : Node
{    
    public static WaterSystem Instance;
    public WaterSystem()
    {
        Instance = this;
    }
    
    private Godot.Collections.Dictionary<Vector3I, WaterUnit> _waterUnitDict = new Godot.Collections.Dictionary<Vector3I, WaterUnit>();

    public WaterUnit GetOrCreateWaterUnit(Vector3I faceCoords)
    {
        if (_waterUnitDict.ContainsKey(faceCoords))
        {
            return _waterUnitDict[faceCoords];
        }
        
        WaterUnit newUnit = new WaterUnit(Faces.Instance.GetFace(faceCoords));
        AddChild(newUnit);
        return _waterUnitDict[faceCoords] = newUnit;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Cells.Instance.CellClick += (cell, vertex) =>
        //{
            //int radius = 0;
            //int N = radius;
            //
            //for (int q = -N; q <= N; q++)
            //{
                //for (int r = -N; r <= N; r++)
                //{
                    //for (int s = -N; s <= N; s++)
                    //{
                        //if (q + r + s == 0)
                        //{
                            //Vector3I cellIndex = new Vector3I(q, r, s) + cell.GetCoords();
                            //Face[] faces = Cells.Instance.GetFaces(cellIndex);
                            //for (int i = 0; i < faces.Length; i++)
                            //{
                                //WaterUnit unit = new WaterUnit(faces[i]);
                                //AddChild(unit);
                            //}
                        //}
                    //}
                //}
            //}
        //};
        
        Faces.Instance.FaceClick += (face) => 
        {
            WaterUnit unit = new WaterUnit(face);
            AddChild(unit);
        };

        // TODO: allow multiple waters to be on a face at once (this is weird but might be fun)
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
