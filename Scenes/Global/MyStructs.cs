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

public struct ElementBean
{
    public int Id;
	public string Name;
    public string Path;

    public ElementBean()
    {
        this.Id = -1;
        this.Name = "Element";
        this.Path = "res://Scenes/Element/Element.tscn";
    }

    public ElementBean(int Id, string Name, string Path)
    {
        this.Id = Id;
        this.Name = Name;
        this.Path = Path;
    }
}

public partial class MyStructs : Node
{
    
}
