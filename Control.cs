using Godot;
using System;


public partial class ViewControl : Node
{
    [Signal]
    public delegate void OnChangeShowVerticesEventHandler(bool showVertices);
    
    public static ViewControl Instance;
    public ViewControl()
    {
        Instance = this;
    }
    
    private bool _showVertices = false;
    
    // Called when the node enters the scene tree for the first time.
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsAction("view_1"))
        {
            _showVertices = false;
        }
        
        if (@event.IsAction("view_4"))
        {
            _showVertices = true;
        }
        
        EmitSignal(SignalName.OnChangeShowVertices, _showVertices);
    }
    
    public bool GetShowVertices()
    {
        return _showVertices;
    }
}

public partial class TileControl : Node
{
    public static TileControl Instance;
    public TileControl()
    {
        Instance = this;
    }
    
    private Vector2I _selectedTile = new Vector2I(0, 0);
    
    // Called when the node enters the scene tree for the first time.
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsAction("material_1"))
        {
            _selectedTile = new Vector2I(0, 0);
            return;
        }
        
        if (@event.IsAction("material_2"))
        {
            _selectedTile = new Vector2I(1, 0);
            return;
        }
    }
    
    public Vector2I GetSelectedTile()
    {
        return _selectedTile;
    }
}

public partial class Control : Node
{
    public static Control Instance;
    
    public Control()
    {
        Instance = this;
    }
    
    private int _selectedControl = 0;
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsAction("material_1"))
        {
            _selectedControl = 0;
            return;
        }
        
        if (@event.IsAction("dig_1"))
        {
            _selectedControl = 1;
            return;
        }
        
        if (@event.IsAction("view_1"))
        {
            _selectedControl = 2;
            return;
        }
    }
    
    public int GetSelectedControl()
    {
        return _selectedControl;
    }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddChild(new TileControl());
        AddChild(new ViewControl());
    }
}
