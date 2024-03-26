using Godot;
using System;

public partial class Paths : Node
{
    public static string GenMapDataPath()
    {
        return ProjectSettings.GlobalizePath("res://Assets/Data/Maps/");
    }

    public static string GenDataPath()
    {
        return ProjectSettings.GlobalizePath("res://Assets/Data/");
    }
}
