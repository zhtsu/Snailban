using Godot;
using System;

public partial class Element : Node2D
{
	[Export]
	public ElementType Type = ElementType.None;
	public int Id = -1;
	public string DisplayName = "Element";
	public Vector2I Location;
	public bool Moving = false;
	public bool CanMove = true;
}
