using System;
using Godot;

[GlobalClass]
public partial class LabelUtils : Godot.Node
{
    public static Node2D CreateLabel(string text)
    {
        var container = new Node2D();
        var label = new Label();
        var theme = (Theme)ResourceLoader.Load("res://new_theme.tres", "Theme");
        var size = theme.DefaultFont.GetStringSize(text);

        label.SetTheme(theme);
        label.SetText(text);

        label.SetPosition(new Vector2(-3, -7));
        label.SetZIndex(1);

        container.AddChild(label);

        return container;
    }
}

public partial class MyLabel : Godot.Node2D
{
    private Label _label;
    public MyLabel(): this(new Label()) {}
    
    public MyLabel(Label label)
    {
        _label = label;
    }
    
    public MyLabel(string text) : this(new Label())
    {
        _label.Text = text;
    }

    
    public void SetText(string text)
    {
        _label.Text = text;
    }
    
    public override void _Ready()
    {
        var theme = (Theme)ResourceLoader.Load("res://new_theme.tres", "Theme");
        var size = theme.DefaultFont.GetStringSize(_label.Text);
        //GD.Print("string size", size);
        _label.SetTheme(theme);
        _label.SetPosition(new Vector2(-3, -7)); // one digit I guess
        _label.SetZIndex(1);

        AddChild(_label);
    }
    
}
