using System.Collections.Generic;
using Godot;

public struct FMapBean
{
    public int Id;
    public string Name;
    public int [,] Matrix;
}

public struct FElementBean
{
    public int Id;
	public string Name;
    public string Path;
    public Texture2D Icon;

    public FElementBean()
    {
        this.Id = -1;
        this.Name = "Element";
        this.Path = "res://Scenes/Element/Element.tscn";
        this.Icon = null;
    }

    public FElementBean(int Id, string Name, string Path)
    {
        this.Id = Id;
        this.Name = Name;
        this.Path = Path;
        this.Icon = null;
    }
}

public struct FRemovedElement
{
    public int Id;
    public Vector2I Location;
}

public struct FOneStep
{
    public Dictionary<Element, Vector2I> MovedElements;
    public bool AnyElementRemoved;
    public List<Element> RemovedElements;

    public FOneStep()
    {
        MovedElements = new Dictionary<Element, Vector2I>();
        AnyElementRemoved = false;
        RemovedElements = new List<Element>();
    }
}

public partial class MyStructs : Node
{
    
}
