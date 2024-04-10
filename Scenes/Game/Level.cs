using Godot;
using System;
using System.Collections.Generic;


public partial class Level : Node2D
{
	[Export]
	public int MapId;
	public FMapBean MapBean;
	private Dictionary<int, PackedScene> PreloadedElementDict = new Dictionary<int, PackedScene>();
	public Player MyPlayer;
    public Element[,] MapMatrix = new Element[8, 8];
	private Element[,] MapMatrixBak = new Element[8, 8];
	private CustomSignals MySignals;
	public Dictionary<int, FOneStep> ElementLocationHistory;
	public int StepCount = 0;
	private Label StepCountLabel;
	private Godot.Collections.Array<TargetPoint> TargetPoints = new Godot.Collections.Array<TargetPoint>();
	private Door MyDoor;
	// For map editor
	// If start a level from map editor
	// Set the SimulationMode to true
	[Export]
	public bool SimulationMode = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ElementLocationHistory = new Dictionary<int, FOneStep>();
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
		CheckAllTargetPoint();

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
				if (ConfigData.ElementBeanDict.TryGetValue(ElementId, out FElementBean MyElementBean) == false)
				{
					continue;
				}

				if (PreloadedElementDict.TryGetValue(ElementId, out PackedScene ElementScene))
				{
					Element MyElement = (Element)ElementScene.Instantiate();
					MapMatrix[i, j] = MyElement;
					MapMatrixBak[i, j] = MyElement;
					MyElement.Id = MyElementBean.Id;
					MyElement.Location = Location;
					MyElement.Name = MyElementBean.Name;
					if (MyElement.Type == ElementType.Player)
					{
						MyPlayer = (Player)MyElement;
					}
					else if (MyElement.Type == ElementType.TargetPoint)
					{
						TargetPoint MyTargetPoint = MyElement as TargetPoint;
						TargetPoints.Add(MyTargetPoint);
						if (MyTargetPoint != null)
						{
							MyTargetPoint.SnailEntered += CheckAllTargetPoint;
							MyTargetPoint.SnailExited += CheckAllTargetPoint;
						}
					}
					else if (MyElement.Type == ElementType.Door)
					{
						MyDoor = (Door)MyElement;
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
				if (ConfigData.ElementBeanDict.TryGetValue(ElementId, out FElementBean MyElementBean) == true &&
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
	public void MoveElement(Element MovedElement, Direction MovementDirection)
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

		if (ElementLocationHistory.TryGetValue(SavedStepCount, out FOneStep OneStep))
		{
			OneStep.MovedElements.Add(MovedElement, OldLocation);
		}
		else
		{
			FOneStep NewOneStep = new FOneStep();
			NewOneStep.MovedElements.Add(MovedElement, OldLocation);
			ElementLocationHistory.Add(SavedStepCount, NewOneStep);
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

		Element OldLocationElementBak = MapMatrixBak[OldLocation.X, OldLocation.Y];
		if (OldLocationElementBak != null && OldLocationElementBak.Type == ElementType.TargetPoint)
		{
			MapMatrix[OldLocation.X, OldLocation.Y] = MapMatrixBak[OldLocation.X, OldLocation.Y];
		}

		MapMatrix[NewLocation.X, NewLocation.Y] = MovedElement;
		MovedElement.Location = NewLocation;

		CreateTween()
		.TweenProperty(MovedElement, "position", NewPosition, 0.2f)
		.SetEase(Tween.EaseType.Out)
		.Connect("finished", Callable.From(() => ResetElementMoving(MovedElement)));
	}

	public Element GetFacingElement(Element CheckedElement, Direction MovementDirection)
	{
		Vector2I TargetLocation = GetTargetLocation(CheckedElement, MovementDirection);
		if (TargetLocation.X < 0 || TargetLocation.X > 7 || TargetLocation.Y < 0 || TargetLocation.Y > 7)
		{
			return null;
		}

		return MapMatrix[TargetLocation.X, TargetLocation.Y];
	}

	// Access and check the element in front of the player
	// If player can move and will return true
	private bool HandleFacingElement(Element CheckedElement, Direction MovementDirection)
	{
		Vector2I TargetLocation = GetTargetLocation(CheckedElement, MovementDirection);
		int X = TargetLocation.X;
		int Y = TargetLocation.Y;
		
		if (X < 0 || X > 7 || Y < 0 || Y > 7)
		{
			return false;
		}

		Element FacingElement = GetFacingElement(MyPlayer, MovementDirection);
		if (FacingElement == null)
		{
			return true;
		}

		if (FacingElement.Type == ElementType.Barrier)
		{
			return false;
		} 
		else if (FacingElement.Type == ElementType.Snail)
		{
			return HandleSnail((Snail)FacingElement, MovementDirection);
		}
		else if (FacingElement.Type == ElementType.Door)
		{
			return HandleDoor((Door)FacingElement);
		}
		
		return true;
	}

	public bool HandleSnail(Snail CheckedSnail, Direction MovementDirection)
	{
		Element FacingElement = GetFacingElement(CheckedSnail, MovementDirection);
		if (FacingElement != null && FacingElement.Type == ElementType.Snail)
		{
			Snail FacingSnail = (Snail)FacingElement;
			if (FacingSnail.Kind == SnailKind.Fire && (CheckedSnail.Kind == SnailKind.Leaf || CheckedSnail.Kind == SnailKind.Water))
			{
				return CheckedSnail.HandleMove(this, MovementDirection);
			}

			return false;
		}
		else if (FacingElement != null && FacingElement.Type == ElementType.Door)
		{
			return false;
		}
		else if (FacingElement != null && FacingElement.Type == ElementType.TargetPoint)
		{
			TargetPoint MyTargetPoint = (TargetPoint)FacingElement;
			if (CheckedSnail.Kind != MyTargetPoint.Kind)
			{
				return false;
			}
			else
			{
				MoveElement(CheckedSnail, MovementDirection);
				return true;
			}
		}

		if (IsElementCanMove(CheckedSnail, MovementDirection))
		{
			MoveElement(CheckedSnail, MovementDirection);
			return CheckedSnail.HandleMove(this, MovementDirection);
		}

		return false;
	}

	private void CheckAllTargetPoint()
	{
		if (MyDoor == null)
		{
			return;
		}

		bool AllCompleted = true;
		foreach (TargetPoint TP in TargetPoints)
		{
			AllCompleted = AllCompleted && TP.Completed;
		}

		if (AllCompleted)
		{
			MyDoor.OpenTheDoor();
		}
		else
		{
			MyDoor.CloseTheDoor();
		}
	}

	public bool HandleDoor(Door CheckedDoor)
	{
		if (CheckedDoor.Accept == true)
		{
			MySignals.EmitSignal("LevelStarted", 1);
			return false;
		}

		return false;
	}

	// If the element can move, return the location after moving
	private bool IsElementCanMove(Element MovedElement, Direction MovementDirection)
	{
		if (MovedElement.CanMove == false)
		{
			return false;
		}

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
		if (MyPlayer.CanMove == false)
		{
			return;
		}

		if (MyPlayer.Moving || HandleFacingElement(MyPlayer, Direction.Up) == false)
		{
			return;
		}

		StepCount += 1;
		StepCountLabel.Text = StepCount.ToString();

		MoveElement(MyPlayer, Direction.Up);
	}

	private void DownKeyDown()
	{
		if (MyPlayer.CanMove == false)
		{
			return;
		}

		if (MyPlayer.Moving || HandleFacingElement(MyPlayer, Direction.Down) == false)
		{
			return;
		}

		StepCount += 1;
		StepCountLabel.Text = StepCount.ToString();

		MoveElement(MyPlayer, Direction.Down);
	}

	private void LeftKeyDown()
	{
		if (MyPlayer.CanMove == false)
		{
			return;
		}

		if (MyPlayer.Moving || HandleFacingElement(MyPlayer, Direction.Left) == false)
		{
			return;
		}

		StepCount += 1;
		StepCountLabel.Text = StepCount.ToString();

		MoveElement(MyPlayer, Direction.Left);
	}

	private void RightKeyDown()
	{
		if (MyPlayer.CanMove == false)
		{
			return;
		}

		if (MyPlayer.Moving || HandleFacingElement(MyPlayer, Direction.Right) == false)
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

		if (ElementLocationHistory.TryGetValue(StepCount, out FOneStep OneStep) == false)
		{
			return;
		}

		foreach (Element Key in OneStep.MovedElements.Keys)
		{
			OneStep.MovedElements.TryGetValue(Key, out Vector2I OldLocation);
			MoveElementByLocation(Key, OldLocation);
			if (Key is Snail)
			{
				Snail RedoSnail = Key as Snail;
				if (RedoSnail != null)
				{
					RedoSnail.HandleRedo(this, OldLocation);
				}
			}
		}

		if (OneStep.RemovedElements.Count > 0)
		{
			foreach (Element RemovedElement in OneStep.RemovedElements)
			{
				AddChild(RemovedElement);
				MapMatrix[RemovedElement.Location.X, RemovedElement.Location.Y] = RemovedElement;
			}
		}

		ElementLocationHistory.Remove(StepCount);
		StepCount -= 1;
		StepCountLabel.Text = StepCount.ToString();
	}

	public void RemoveElement(Element RemovedElement)
	{
		RemovedElement.CanMove = false;
		MyPlayer.CanMove = false;
		AnimationPlayer ElementAnimPlayer = RemovedElement.GetNode<AnimationPlayer>("AnimationPlayer");
		ElementAnimPlayer.Play("Flicker");

		RemovalElementQueue.Enqueue(RemovedElement);

		if (ElementAnimPlayer.IsConnected("animation_finished", new Callable(this, nameof(RemoveElementCallable))))
		{
			ElementAnimPlayer.Disconnect("animation_finished", new Callable(this, nameof(RemoveElementCallable)));
		}
		ElementAnimPlayer.Connect("animation_finished", new Callable(this, nameof(RemoveElementCallable)));

		int SavedStepCount = StepCount + 1;

		if (ElementLocationHistory.TryGetValue(SavedStepCount, out FOneStep OneStep))
		{
			OneStep.RemovedElements.Add(RemovedElement);
		}
		else
		{
			FOneStep NewOneStep = new FOneStep();
			NewOneStep.RemovedElements.Add(RemovedElement);
			ElementLocationHistory.Add(SavedStepCount, NewOneStep);
		}
	}

	private Queue<Element> RemovalElementQueue = new Queue<Element>();
	private void RemoveElementCallable(string AnimName)
	{
		List<Vector2> ExplosionPositions = new List<Vector2>();
		foreach (Element RemovedElement in RemovalElementQueue)
		{
			ExplosionPositions.Add(RemovedElement.Position);
			RemoveChild(RemovedElement);
			MapMatrix[RemovedElement.Location.X, RemovedElement.Location.Y] = null;
		}
		RemovalElementQueue.Clear();
		
		Explosion FirstExplosion = null;
		for (int i = 0; i < ExplosionPositions.Count; i++)
		{
			Vector2 Position = ExplosionPositions[i];
			PackedScene ExplosionScene = (PackedScene)GD.Load("res://Scenes/Game/Explosion.tscn");
			Explosion MyExplosion = (Explosion)ExplosionScene.Instantiate();
			MyExplosion.Position = Position;
			AddChild(MyExplosion);

			if (i == 0)
			{
				FirstExplosion = MyExplosion;
			}
		}

		if (FirstExplosion != null)
		{
			FirstExplosion.ExplosionFinished += () => {
				MyPlayer.CanMove = true;
			};
		}
	}
}
