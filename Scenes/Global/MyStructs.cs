using Godot;

public struct FMapBean
{
    public int Id;
    public string Name;
    public int Row;
    public int Column;
    public int TileWidth;
    public int TileHeight;
    public int LayerCount;
    public int [,,] Layers;
}

public partial class MyStructs : Node
{
    
}
