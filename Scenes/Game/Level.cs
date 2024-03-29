using Godot;
using System;
using System.Collections.Generic;


public partial class Level : Node2D
{
	[Export]
	public int MapId;
	private FMapBean MapBean;
	private Dictionary<int, PackedScene> PreloadedElementDict = new Dictionary<int, PackedScene>();
	private Player MyPlayer;
	private CustomSignals MySignals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Initialize the map bean
		// The variable 'MapBean' will updated when the game state was changed
		if (ConfigData.MapBeanDict.TryGetValue(MapId, out MapBean) == false)
		{
			GD.PushWarning("Invalid map id! id: " + MapId.ToString());
			return;
		}

		PreloadElement();
		InitMap();

		MySignals = GetNode<CustomSignals>("/root/CustomSignals");
		MySignals.UpKey += UpKeyDown;
		MySignals.DownKey += DownKeyDown;
		MySignals.LeftKey += LeftKeyDown;
		MySignals.RightKey += RightKeyDown;
		MySignals.SpaceKey += SpaceKeyDown;
	}

    public override void _ExitTree()
    {
        base._ExitTree();

		MySignals.UpKey -= UpKeyDown;
		MySignals.DownKey -= DownKeyDown;
		MySignals.LeftKey -= LeftKeyDown;
		MySignals.RightKey -= RightKeyDown;
		MySignals.SpaceKey -= SpaceKeyDown;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	private void InitMap()
	{
		System.Numerics.Vector2 Location = new System.Numerics.Vector2(0, 0);

		for (int i = 0; i < MapBean.LayerCount; i++)
		{
			for (int j = 0; j < MapBean.Row; j++)
			{
				Location.X = j;
				for (int k = 0; k < MapBean.Column; k++)
				{
					Location.Y = k;
					int ElementId = MapBean.Layers[i, j, k];
					if (ConfigData.ElementBeanDict.TryGetValue(ElementId, out ElementBean MyElementBean) == false)
					{
						continue;
					}

					if (PreloadedElementDict.TryGetValue(ElementId, out PackedScene ElementScene))
					{
						Element MyElement = (Element)ElementScene.Instantiate();
						MyElement.Id = MyElementBean.Id;
						MyElement.Name = MyElementBean.Name;
						if (MyElementBean.Name == "Player")
						{
							MyPlayer = (Player)MyElement;
							MyPlayer.Location = Location;
						}
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
					if (ConfigData.ElementBeanDict.TryGetValue(ElementId, out ElementBean MyElementBean) == true &&
					    PreloadedElementDict.TryGetValue(ElementId, out PackedScene Scene) == false)
					{
						PackedScene MyElementScene = (PackedScene)GD.Load(ProjectSettings.GlobalizePath(MyElementBean.Path));
						PreloadedElementDict.Add(ElementId, MyElementScene);
					}
				}
			}
		}
	}

	private void UpKeyDown()
	{
		GD.Print("UP");
	}

	private void DownKeyDown()
	{
		GD.Print("Down");
	}

	private void LeftKeyDown()
	{
		GD.Print("LEFT");
	}

	private void RightKeyDown()
	{
		GD.Print("RIGHT");
	}

	private void SpaceKeyDown()
	{
		GD.Print("SPACE");
	}
}
