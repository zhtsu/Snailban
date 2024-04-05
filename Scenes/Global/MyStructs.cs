using Godot;

public struct FMapBean
{
    public int Id;
    public string Name;
    public int [,] Matrix;
}

public struct ElementBean
{
    public int Id;
	public string Name;
    public string Path;
    public Texture2D Icon;

    public ElementBean()
    {
        this.Id = -1;
        this.Name = "Element";
        this.Path = "res://Scenes/Element/Element.tscn";
        this.Icon = null;
    }

    public ElementBean(int Id, string Name, string Path)
    {
        this.Id = Id;
        this.Name = Name;
        this.Path = Path;
        this.Icon = null;
    }
}

public partial class MyStructs : Node
{
    
}
