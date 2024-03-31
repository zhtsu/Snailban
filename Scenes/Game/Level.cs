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
    private Element[,] MapMatrix = new Element[8, 8];
	// Uecord the initial value of the map
	// Used to recover the Matrix's value when the player leave from a location
	private Element[,] MapMatrixBak = new Element[8, 8];
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
						MapMatrix[j, k] = MyElement;
						MapMatrixBak[j, k] = MyElement;
						MyElement.Id = MyElementBean.Id;
						MyElement.Name = MyElementBean.Name;
						if (MyElementBean.Name == "Player")
						{
							MyPlayer = (Player)MyElement;
							MyPlayer.Location = Location;
							MapMatrixBak[j, k] = null;
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

	private void UpdatePlayerPosition(System.Numerics.Vector2 OldLocation)
	{
		Vector2 NewPosition = new Vector2(
			MyPlayer.Location.X * 64,
			MyPlayer.Location.Y * 64
		);

		MapMatrix[(int)OldLocation.X, (int)OldLocation.Y] = MapMatrixBak[(int)OldLocation.X, (int)OldLocation.Y];
		MapMatrix[(int)MyPlayer.Location.X, (int)MyPlayer.Location.Y] = MyPlayer;

		CreateTween()
		.TweenProperty(MyPlayer, "position", NewPosition, 0.2f)
		.SetEase(Tween.EaseType.Out)
		.Connect("finished", new Callable(this, nameof(ResetPlayerMoving)));
	}

	private bool GetFacedElement(Direction MovementDirection, out Element FacingElement)
	{
		int X = (int)MyPlayer.Location.X;
		int Y = (int)MyPlayer.Location.Y;
		switch (MovementDirection)
		{
			case Direction.Up: 		Y -= 1; break;
			case Direction.Down:	Y += 1; break;
			case Direction.Left:	X -= 1; break;
			case Direction.Right:	X += 1; break;
		}
		
		if (X < 0 || X > 7 || Y < 0 || Y > 7)
		{
			FacingElement = null;
			return false;
		}

		FacingElement = MapMatrix[X, Y];
		GD.Print(FacingElement);
		return true;
	}

	// Access and check the element in front of the player
	// If player can move and will return true
	private bool HandleFacedElement(Direction MovementDirection)
	{
		if (GetFacedElement(MovementDirection, out Element FacingElement) == false)
		{
			return false;
		}

		if (FacingElement != null && FacingElement.Type == ElementType.Barrier)
		{
			return false;
		}
		
		return true;
	}

	private void UpKeyDown()
	{
		if (MyPlayer.Moving || HandleFacedElement(Direction.Up) == false)
		{
			return;
		}

		System.Numerics.Vector2 OldLocation = MyPlayer.Location;
		MyPlayer.Location.Y -= 1;
		MyPlayer.Moving = true;
		UpdatePlayerPosition(OldLocation);
	}

	private void DownKeyDown()
	{
		if (MyPlayer.Moving || HandleFacedElement(Direction.Down) == false)
		{
			return;
		}

		System.Numerics.Vector2 OldLocation = MyPlayer.Location;
		MyPlayer.Location.Y += 1;
		MyPlayer.Moving = true;
		UpdatePlayerPosition(OldLocation);
	}

	private void LeftKeyDown()
	{
		if (MyPlayer.Moving || HandleFacedElement(Direction.Left) == false)
		{
			return;
		}

		System.Numerics.Vector2 OldLocation = MyPlayer.Location;
		MyPlayer.Location.X -= 1;
		MyPlayer.Moving = true;
		UpdatePlayerPosition(OldLocation);
	}

	private void RightKeyDown()
	{
		if (MyPlayer.Moving || HandleFacedElement(Direction.Right) == false)
		{
			return;
		}

		System.Numerics.Vector2 OldLocation = MyPlayer.Location;
		MyPlayer.Location.X += 1;
		MyPlayer.Moving = true;
		UpdatePlayerPosition(OldLocation);
	}

	private void ResetPlayerMoving()
	{
		MyPlayer.Moving = false;
	}

	private void SpaceKeyDown()
	{
		GD.Print("SPACE");
	}
}
