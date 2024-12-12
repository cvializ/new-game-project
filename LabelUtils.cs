using Godot;
using System;

[GlobalClass]
public partial class LabelUtils : Godot.Node
{
    public static Node2D CreateLabel(String text)
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
