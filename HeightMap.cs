using Godot;
using System;

public partial class HeightMap : Node
{
    private static Vector2I DefaultDimensions = new Vector2I(512, 512);
    Vector2I _dimensions = DefaultDimensions;
    Image _image = Image.CreateEmpty(DefaultDimensions.X, DefaultDimensions.Y, false, Image.Format.Rgba8); 

    public HeightMap() {}

    public HeightMap(string path)
    {
        _image = Image.LoadFromFile(path);
    }

    public Image GetImage()
    {
        return _image;
    }
}
