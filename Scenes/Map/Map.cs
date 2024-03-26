using Godot;
using System;
using System.Collections.Generic;


public partial class Map : Node2D
{
	[Export]
	public int Id;

	private FMapData Data;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (GData.MapDataDict.TryGetValue(Id, out Data) == false)
		{
			GD.PushWarning("Invalid ID of map! ID: " + Id.ToString());
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void InitLayers()
	{
		
	}

	private void Refresh()
	{

	}
}
