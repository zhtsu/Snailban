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

	private void InitMap()
	{
		//PackedScene TscnScene = GD.Load(Paths.GenElementPath(""));

		for (int i = Data.LayerCount - 1; i >= 0; i--)
		{
			for (int j = 0; j < Data.Row; j++)
			{
				for (int k = 0; k < Data.Column; k++)
				{
					int ElementId = Data.Layers[i, j, k];
					if (GData.ElementDict.TryGetValue(ElementId, out string TscnPath))
					{
						//AddChild();
					}
				}
			}
		}
	}
}
