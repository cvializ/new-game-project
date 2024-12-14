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
    private string _text;
    public MyLabel(): this("[todo]") {}
    
    public MyLabel(string text)
    {
        this._text = text;
    }
    
    public override void _Ready()
    {
        var label = new Label();
        var theme = (Theme)ResourceLoader.Load("res://new_theme.tres", "Theme");
        var size = theme.DefaultFont.GetStringSize(_text);

        label.SetTheme(theme);
        label.SetText(_text);

        label.SetPosition(new Vector2(-3, -7));
        label.SetZIndex(1);

        this.AddChild(label);
    }
    
}


public partial class MyOffsetLabel : Godot.Node2D
{
    private string _text;
    public MyOffsetLabel(): this("[todo]") {}
    public MyOffsetLabel(string text)
    {
        this._text = text;
    }
    
    public override void _Ready()
    {
        var label = new Label();
        var theme = (Theme)ResourceLoader.Load("res://new_theme.tres", "Theme");
        var size = theme.DefaultFont.GetStringSize(_text);

        label.SetTheme(theme);
        label.SetText(_text);

        label.SetPosition(new Vector2(110, -7));
        label.SetZIndex(1);

        this.AddChild(label);
    }
    
}
