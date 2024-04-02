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
	private List<Dictionary<Element, Vector2>> ElementLocationHistory;

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

	private Vector2I GetTargetLocation(Element MovedElement, Direction MovementDirection)
	{
		Vector2I NewLocation = (Vector2I)MovedElement.Location;

		switch (MovementDirection)
		{
			case Direction.Up: 		NewLocation.Y -= 1; break;
			case Direction.Down:	NewLocation.Y += 1; break;
			case Direction.Left:	NewLocation.X -= 1; break;
			case Direction.Right:	NewLocation.X += 1; break;
		}

		return NewLocation;
	}

	// This function will move the element without any check
	// Make sure the required check was completed when call this function
	private void MoveElement(Element MovedElement, Direction MovementDirection)
	{
		MovedElement.Moving = true;

		Vector2I OldLocation = (Vector2I)MovedElement.Location;
		Vector2I NewLocation = GetTargetLocation(MovedElement, MovementDirection);

		Vector2 NewPosition = new Vector2(NewLocation[0] * 64, NewLocation[1] * 64);
		
		MapMatrix[OldLocation.X, OldLocation.Y] = null;
		MapMatrix[NewLocation.X, NewLocation.Y] = MovedElement;
		MovedElement.Location = new Vector2(NewLocation[0], NewLocation[1]);

		CreateTween()
		.TweenProperty(MovedElement, "position", NewPosition, 0.2f)
		.SetEase(Tween.EaseType.Out)
		.Connect("finished", Callable.From(() => ResetElementMoving(MovedElement)));
	}

	private Element GetFacedElement(Element CheckedElement, Direction MovementDirection)
	{
		Vector2I TargetLocation = GetTargetLocation(CheckedElement, MovementDirection);
		return MapMatrix[TargetLocation.X, TargetLocation.Y];
	}

	// Access and check the element in front of the player
	// If player can move and will return true
	private bool HandleFacedElement(Element CheckedElement, Direction MovementDirection)
	{
		Vector2I TargetLocation = GetTargetLocation(CheckedElement, MovementDirection);
		int X = TargetLocation.X;
		int Y = TargetLocation.Y;
		
		if (X < 0 || X > 7 || Y < 0 || Y > 7)
		{
			return false;
		}

		Element FacingElement = GetFacedElement(MyPlayer, MovementDirection);
		if (FacingElement != null && FacingElement.Type == ElementType.Barrier)
		{
			return false;
		} 
		else if (FacingElement != null && FacingElement.Type == ElementType.Snail)
		{
			return HandleSnail((Snail)FacingElement, MovementDirection);
		}
		
		return true;
	}

	public bool HandleSnail(Snail FacingSnail, Direction MovementDirection)
	{
		if (IsElementCanMove(FacingSnail, MovementDirection))
		{
			MoveElement(FacingSnail, MovementDirection);
			return true;
		}

		return false;
	}

	// If the element can move, return the location after moving
	private bool IsElementCanMove(Element MovedElement, Direction MovementDirection)
	{
		Vector2I TargetLocation = GetTargetLocation(MovedElement, MovementDirection);
		int X = TargetLocation.X;
		int Y = TargetLocation.Y;
		
		if (X < 0 || X > 7 || Y < 0 || Y > 7)
		{
			return false;
		}

		Element FacedElement = GetFacedElement(MovedElement, MovementDirection);
		if (FacedElement != null && FacedElement.Type == ElementType.Barrier)
		{
			return false;
		}

		return true;
	}

	private void UpKeyDown()
	{
		if (MyPlayer.Moving || HandleFacedElement(MyPlayer, Direction.Up) == false)
		{
			return;
		}

		MoveElement(MyPlayer, Direction.Up);
	}

	private void DownKeyDown()
	{
		if (MyPlayer.Moving || HandleFacedElement(MyPlayer, Direction.Down) == false)
		{
			return;
		}

		MoveElement(MyPlayer, Direction.Down);
	}

	private void LeftKeyDown()
	{
		if (MyPlayer.Moving || HandleFacedElement(MyPlayer, Direction.Left) == false)
		{
			return;
		}

		MoveElement(MyPlayer, Direction.Left);
	}

	private void RightKeyDown()
	{
		if (MyPlayer.Moving || HandleFacedElement(MyPlayer, Direction.Right) == false)
		{
			return;
		}

		MoveElement(MyPlayer, Direction.Right);
	}

	private void ResetElementMoving(Element MovedElement)
	{
		MovedElement.Moving = false;
	}

	private void SpaceKeyDown()
	{
		GD.Print("SPACE");
	}
}
