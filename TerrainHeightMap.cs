using Godot;
using System;

public partial class TerrainHeightMap : Node
{
    public static TerrainHeightMap Instance;
    
    private HeightMap _heightMap;
    
    public TerrainHeightMap()
    {
        Instance = this;
        
        _heightMap = new HeightMap("./heightmap_sm.png");
        
        var image = _heightMap.GetImage();
        if (image.GetWidth() % 3 > 0) 
        {
            GD.Print("ERROR: Image width should be multiple of 3");
        }
        if (image.GetHeight() % 2 > 0)
        {
            GD.Print("ERROR: Image height should be multiple of 2");
        }
    }
    
    public HeightMap GetHeightMap()
    {
        return _heightMap;
    }
}

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
    
    public Vector2I GetSize()
    {
        return _image.GetSize();
    }
    
    public int GetHeightAtPixel(Vector2I pixelCoords, int maxHeight = 20)
    {
        Color pixel = _image.GetPixelv(pixelCoords);
        return (int)Math.Round(pixel.Luminance * maxHeight);
    }
}
