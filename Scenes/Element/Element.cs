using Godot;
using System;

public partial class Element : Node2D
{
	public int Id = -1;
	public string DisplayName = "Element";
	public ElementType Type = ElementType.None;
	public Vector2 Location;
}
