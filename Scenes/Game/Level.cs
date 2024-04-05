using Godot;
using System;
using System.Collections.Generic;


public partial class Level : Node2D
{
	[Export]
	public int MapId;
	public FMapBean MapBean;
	private Dictionary<int, PackedScene> PreloadedElementDict = new Dictionary<int, PackedScene>();
	private Player MyPlayer;
    private Element[,] MapMatrix = new Element[8, 8];
	private CustomSignals MySignals;
	private Dictionary<int, Dictionary<Element, Vector2I>> ElementLocationHistory;
	private int StepCount = 0;
	private Label StepCountLabel;
	// For map editor
	// If start a level from map editor
	// Set the SimulationMode to true
	[Export]
	public bool SimulationMode = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ElementLocationHistory = new Dictionary<int, Dictionary<Element, Vector2I>>();
		StepCountLabel = GetNode<Label>("CanvasLayer/StepCount");

		// If level is not simulating in map editor
		// Will auto set the MapBean
		// If SimulationMode was true, make sure the MapBean was set manual
		if (SimulationMode == false)
		{
			// Initialize the map bean
			// The variable 'MapBean' will updated when the game state was changed
			if (ConfigData.MapBeanDict.TryGetValue(MapId, out MapBean) == false)
			{
				GD.PushWarning("Invalid map id! id: " + MapId.ToString());
				return;
			}
		}

		PreloadElement();
		InitMap();

		// If level is simulating in map editor
		// Use key event in _Input()
		if (SimulationMode == false)
		{
			MySignals = GetNode<CustomSignals>("/root/CustomSignals");
			MySignals.UpKey += UpKeyDown;
			MySignals.DownKey += DownKeyDown;
			MySignals.LeftKey += LeftKeyDown;
			MySignals.RightKey += RightKeyDown;
			MySignals.SpaceKey += SpaceKeyDown;
		}
	}

    public override void _ExitTree()
    {
        base._ExitTree();

		if (SimulationMode == false)
		{
			MySignals.UpKey -= UpKeyDown;
			MySignals.DownKey -= DownKeyDown;
			MySignals.LeftKey -= LeftKeyDown;
			MySignals.RightKey -= RightKeyDown;
			MySignals.SpaceKey -= SpaceKeyDown;
		}
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

		if (SimulationMode == false)
		{
			return;
		}

		// For map editor
		if (@event is InputEventKey EventKey)
		{
			if (Input.IsActionJustPressed("Up"))
			{
				UpKeyDown();
			}
			else if (Input.IsActionJustPressed("Down"))
			{
				DownKeyDown();
			}
			else if (Input.IsActionJustPressed("Left"))
			{
				LeftKeyDown();
			}
			else if (Input.IsActionJustPressed("Right"))
			{
				RightKeyDown();
			}
			else if (Input.IsActionJustPressed("Space"))
			{
				SpaceKeyDown();
			}
		}
    }

	private void InitMap()
	{
		Vector2I Location = new Vector2I(0, 0);

		for (int i = 0; i < 8; i++)
		{
			Location.X = i;
			for (int j = 0; j < 8; j++)
			{
				Location.Y = j;
				int ElementId = MapBean.Matrix[i, j];
				if (ConfigData.ElementBeanDict.TryGetValue(ElementId, out ElementBean MyElementBean) == false)
				{
					continue;
				}

				if (PreloadedElementDict.TryGetValue(ElementId, out PackedScene ElementScene))
				{
					Element MyElement = (Element)ElementScene.Instantiate();
					MapMatrix[i, j] = MyElement;
					MyElement.Id = MyElementBean.Id;
					MyElement.Location = Location;
					MyElement.Name = MyElementBean.Name;
					if (MyElementBean.Name == "Player")
					{
						MyPlayer = (Player)MyElement;
					}
					MyElement.Position = new Vector2(j * 64, i * 64);
					AddChild(MyElement);
				}
			}
		}
	}

	private void PreloadElement()
	{
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				int ElementId = MapBean.Matrix[i, j];
				if (ConfigData.ElementBeanDict.TryGetValue(ElementId, out ElementBean MyElementBean) == true &&
					PreloadedElementDict.TryGetValue(ElementId, out PackedScene Scene) == false)
				{
					PackedScene MyElementScene = (PackedScene)GD.Load(ProjectSettings.GlobalizePath(MyElementBean.Path));
					PreloadedElementDict.Add(ElementId, MyElementScene);
				}
			}
		}
	}

	private Vector2I GetTargetLocation(Element MovedElement, Direction MovementDirection)
	{
		Vector2I NewLocation = (Vector2I)MovedElement.Location;

		switch (MovementDirection)
		{
			case Direction.Up: 		NewLocation.X -= 1; break;
			case Direction.Down:	NewLocation.X += 1; break;
			case Direction.Left:	NewLocation.Y -= 1; break;
			case Direction.Right:	NewLocation.Y += 1; break;
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

		int SavedStepCount = StepCount;
		if (MovedElement.Type == ElementType.Snail)
		{
			// Because the snail's movable check precedes player's
			// And when the snail can move, mean the player can also move
			// When the player moved(After current operation), we need to let StepCount + 1
			// So let the StepCount + 1 now to make sure the StepCount is the same for both
			SavedStepCount += 1;
		}

		if (ElementLocationHistory.TryGetValue(SavedStepCount, out Dictionary<Element, Vector2I> AllMovedElement))
		{
			AllMovedElement.Add(MovedElement, OldLocation);
		}
		else
		{
			Dictionary<Element, Vector2I> Dict = new Dictionary<Element, Vector2I>();
			Dict.Add(MovedElement, OldLocation);
			ElementLocationHistory.Add(SavedStepCount, Dict);
		}

		MoveElementByLocation(MovedElement, NewLocation);
	}

	private void MoveElementByLocation(Element MovedElement, Vector2I NewLocation)
	{
		Vector2 NewPosition = new Vector2(NewLocation.Y * 64, NewLocation.X * 64);
		Vector2I OldLocation = MovedElement.Location;

		// When redo, the old location may have been covered by a snail element
		// Only the element in old location is itself(Not a snail), it can be set to null
		if (MapMatrix[OldLocation.X, OldLocation.Y] == MovedElement)
		{
			MapMatrix[OldLocation.X, OldLocation.Y] = null;
		}
		MapMatrix[NewLocation.X, NewLocation.Y] = MovedElement;
		MovedElement.Location = NewLocation;

		CreateTween()
		.TweenProperty(MovedElement, "position", NewPosition, 0.2f)
		.SetEase(Tween.EaseType.Out)
		.Connect("finished", Callable.From(() => ResetElementMoving(MovedElement)));
	}

	private Element GetFacingElement(Element CheckedElement, Direction MovementDirection)
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

		Element FacingElement = GetFacingElement(MyPlayer, MovementDirection);
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

	public bool HandleSnail(Snail CheckedSnail, Direction MovementDirection)
	{
		Element FacingElement = GetFacingElement(CheckedSnail, MovementDirection);
		if (FacingElement != null && FacingElement.Type == ElementType.Snail)
		{
			return false;
		}

		if (IsElementCanMove(CheckedSnail, MovementDirection))
		{
			MoveElement(CheckedSnail, MovementDirection);
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

		Element FacedElement = GetFacingElement(MovedElement, MovementDirection);
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

		StepCount += 1;
		StepCountLabel.Text = StepCount.ToString();

		MoveElement(MyPlayer, Direction.Up);
	}

	private void DownKeyDown()
	{
		if (MyPlayer.Moving || HandleFacedElement(MyPlayer, Direction.Down) == false)
		{
			return;
		}

		StepCount += 1;
		StepCountLabel.Text = StepCount.ToString();

		MoveElement(MyPlayer, Direction.Down);
	}

	private void LeftKeyDown()
	{
		if (MyPlayer.Moving || HandleFacedElement(MyPlayer, Direction.Left) == false)
		{
			return;
		}

		StepCount += 1;
		StepCountLabel.Text = StepCount.ToString();

		MoveElement(MyPlayer, Direction.Left);
	}

	private void RightKeyDown()
	{
		if (MyPlayer.Moving || HandleFacedElement(MyPlayer, Direction.Right) == false)
		{
			return;
		}

		StepCount += 1;
		StepCountLabel.Text = StepCount.ToString();

		MoveElement(MyPlayer, Direction.Right);
	}

	private void ResetElementMoving(Element MovedElement)
	{
		MovedElement.Moving = false;
	}

	private void SpaceKeyDown()
	{
		if (ElementLocationHistory.Count == 0)
		{
			return;
		}

		if (ElementLocationHistory.TryGetValue(StepCount, out Dictionary<Element, Vector2I> AllMovedElement) == false)
		{
			return;
		}

		foreach (Element Key in AllMovedElement.Keys)
		{
			AllMovedElement.TryGetValue(Key, out Vector2I OldLocation);
			MoveElementByLocation(Key, OldLocation);
		}

		ElementLocationHistory.Remove(StepCount);
		StepCount -= 1;
		StepCountLabel.Text = StepCount.ToString();
	}
}
