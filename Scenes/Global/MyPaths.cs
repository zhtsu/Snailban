using Godot;
using System;
using System.Collections.Generic;

public partial class MyPaths : Node
{
    public static string GenMapDataPath(string File)
    {
        return "res://Assets/Data/Maps/" + File;
    }

    public static string GenDataPath(string File)
    {
        return "res://Assets/Data/" + File;
    }

    public static string GenElementPath(string File)
    {
        return "res://Scenes/Element/" + File;
    }

    public static string GenTexturePath(string File)
    {
        return "res://Assets/Textures/" + File;
    }
}
