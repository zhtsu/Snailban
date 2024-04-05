using Godot;
using System;


public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public enum ElementType
{
    None,
    Player,
    Snail,
    Barrier,
    Door,
    TargetPoint
}

public enum SnailKind
{
    None,
    Normal,
    Rainbow,
    Fire,
    Dark,
    Water,
    Noble,
    Leaf,
    Metal
}

public partial class MyEnums : Node
{
	
}
