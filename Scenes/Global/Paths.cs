using Godot;
using System;

public partial class Paths : Node
{
    public static string GenMapDataPath(string File)
    {
        return ProjectSettings.GlobalizePath("res://Assets/Data/Maps/") + File;
    }

    public static string GenDataPath(string File)
    {
        return ProjectSettings.GlobalizePath("res://Assets/Data/") + File;
    }

    public static string GenElementPath(string File)
    {
        return ProjectSettings.GlobalizePath("res://Assets/Scene/Element/") + File;
    }
}
