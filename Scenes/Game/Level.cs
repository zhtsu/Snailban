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
		Vector2 Location = new Vector2(0, 0);

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
						MyElement.Location = Location;
						MyElement.Name = MyElementBean.Name;
						if (MyElementBean.Name == "Player")
						{
							MyPlayer = (Player)MyElement;
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

	private void UpdateElementPosition(Element MovedElement, Vector2 OldLocation, Vector2 NewLocation)
	{
		Vector2 NewPosition = NewLocation * 64;
		
		if (MovedElement.Type == ElementType.Effect)
		{
			MapMatrix[(int)OldLocation.X, (int)OldLocation.Y] = MapMatrixBak[(int)OldLocation.X, (int)OldLocation.Y];
		}
		else
		{
			MapMatrix[(int)OldLocation.X, (int)OldLocation.Y] = null;
		}
		MapMatrix[(int)NewLocation.X, (int)NewLocation.Y] = MovedElement;

		if (MovedElement.Type == ElementType.Player)
		{
			CreateTween()
			.TweenProperty(MovedElement, "position", NewPosition, 0.2f)
			.SetEase(Tween.EaseType.Out)
			.Connect("finished", new Callable(this, nameof(ResetPlayerMoving)));
		}
		else
		{
			CreateTween()
			.TweenProperty(MovedElement, "position", NewPosition, 0.2f)
			.SetEase(Tween.EaseType.Out);
		}
	}

	private bool GetFacedElement(Element CheckedElement, Direction MovementDirection, out Element FacingElement)
	{
		if (IsElementCanMove(CheckedElement, MovementDirection, out int X, out int Y) == false)
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
		if (GetFacedElement(MyPlayer, MovementDirection, out Element FacingElement) == false)
		{
			return false;
		}

		if (FacingElement != null && FacingElement.Type == ElementType.Barrier)
		{
			return false;
		} else if (FacingElement != null && FacingElement.Type == ElementType.Snail)
		{
			return HandleSnail((Snail)FacingElement, MovementDirection);
		}
		
		return true;
	}

	public bool HandleSnail(Snail FacingSnail, Direction MovementDirection)
	{
		return false;
	}

	// If the element can move, will return the location after moving
	private bool IsElementCanMove(Element MovedElement, Direction MovementDirection, out int TargetX, out int TargetY)
	{
		TargetX = (int)MovedElement.Location.X;
		TargetY = (int)MovedElement.Location.Y;
		switch (MovementDirection)
		{
			case Direction.Up: 		TargetY -= 1; break;
			case Direction.Down:	TargetY += 1; break;
			case Direction.Left:	TargetX -= 1; break;
			case Direction.Right:	TargetX += 1; break;
		}
		
		if (TargetX < 0 || TargetX > 7 || TargetY < 0 || TargetY > 7)
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

		Vector2 OldLocation = MyPlayer.Location;
		MyPlayer.Location.Y -= 1;
		MyPlayer.Moving = true;
		UpdateElementPosition(MyPlayer, OldLocation, MyPlayer.Location);
	}

	private void DownKeyDown()
	{
		if (MyPlayer.Moving || HandleFacedElement(Direction.Down) == false)
		{
			return;
		}

		Vector2 OldLocation = MyPlayer.Location;
		MyPlayer.Location.Y += 1;
		MyPlayer.Moving = true;
		UpdateElementPosition(MyPlayer, OldLocation, MyPlayer.Location);
	}

	private void LeftKeyDown()
	{
		if (MyPlayer.Moving || HandleFacedElement(Direction.Left) == false)
		{
			return;
		}

		Vector2 OldLocation = MyPlayer.Location;
		MyPlayer.Location.X -= 1;
		MyPlayer.Moving = true;
		UpdateElementPosition(MyPlayer, OldLocation, MyPlayer.Location);
	}

	private void RightKeyDown()
	{
		if (MyPlayer.Moving || HandleFacedElement(Direction.Right) == false)
		{
			return;
		}

		Vector2 OldLocation = MyPlayer.Location;
		MyPlayer.Location.X += 1;
		MyPlayer.Moving = true;
		UpdateElementPosition(MyPlayer, OldLocation, MyPlayer.Location);
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
