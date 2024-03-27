using Godot;
using System;
using System.Collections.Generic;


public partial class Level : Node2D
{
	[Export]
	public int Id;
	private FMapBean MapBean;
	private Dictionary<int, PackedScene> PreloadedElementDict = new Dictionary<int, PackedScene>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (PreloadedData.MapDataDict.TryGetValue(Id, out MapBean) == false)
		{
			GD.PushWarning("Invalid ID of map! ID: " + Id.ToString());
			return;
		}

		PreloadElement();
		InitMap();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void InitMap()
	{
		for (int i = 0; i < MapBean.LayerCount; i++)
		{
			for (int j = 0; j < MapBean.Row; j++)
			{
				for (int k = 0; k < MapBean.Column; k++)
				{
					int ElementId = MapBean.Layers[i, j, k];
					if (PreloadedElementDict.TryGetValue(ElementId, out PackedScene ElementScene))
					{
						Element MyElement = (Element)ElementScene.Instantiate();
						MyElement.Position = new Vector2(MapBean.TileWidth * j, MapBean.TileHeight * k);
						AddChild(MyElement);
					}
				}
			}
		}
	}

	private void PreloadElement()
	{
		for (int i = MapBean.LayerCount - 1; i >= 0; i--)
		{
			for (int j = 0; j < MapBean.Row; j++)
			{
				for (int k = 0; k < MapBean.Column; k++)
				{
					int ElementId = MapBean.Layers[i, j, k];
					if (PreloadedData.ElementDict.TryGetValue(ElementId, out string ScenePath) == true &&
					    PreloadedElementDict.TryGetValue(ElementId, out PackedScene Scene) == false)
					{
						PreloadedElementDict.Add(ElementId, (PackedScene)GD.Load(ProjectSettings.GlobalizePath(ScenePath)));
					}
				}
			}
		}
	}
}
