using Godot;

public struct FMapData
{
    public int Id;
    public string Name;
    public int Row;
    public int Column;
    public int TileWidth;
    public int TileHeight;
    public int [,,] Layers;
}

public partial class Structs : Node
{
    
}
