using Godot;
using System.Collections.Generic;


public partial class Level : Node2D
{
	[Export]
	public int MapId;
	public FMapBean MapBean;
	private Dictionary<int, PackedScene> PreloadedElementDict = new Dictionary<int, PackedScene>();
	public List<Player> MyPlayers = new List<Player>();
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
	public bool CanRedo = true;

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
			MySignals.Restart += Restart;
		}

		FileAccess UserDataFile = FileAccess.Open(ConfigData.user_data_path, FileAccess.ModeFlags.Write);
		Godot.Collections.Dictionary UserData = new Godot.Collections.Dictionary();
		UserData["last_level"] = MapId;
		UserDataFile.StoreString(UserData.ToString());
		UserDataFile.Close();
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
			MySignals.Restart -= Restart;
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
						MyPlayers.Add((Player)MyElement);
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
					MyElement.Position = new Vector2(j * 64 + 32, i * 64 + 32);
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

	private Vector2I GetTargetLocation(Element MovedElement, Direction MovementDirection, int Step = 1)
	{
		Vector2I NewLocation = (Vector2I)MovedElement.Location;

		switch (MovementDirection)
		{
			case Direction.Up: 		NewLocation.X -= Step; break;
			case Direction.Down:	NewLocation.X += Step; break;
			case Direction.Left:	NewLocation.Y -= Step; break;
			case Direction.Right:	NewLocation.Y += Step; break;
		}

		return NewLocation;
	}

	// This function will move the element without any check
	// Make sure the required check was completed when call this function
	public void MoveElement(Element MovedElement, Direction MovementDirection, int Step = 1, bool UseZoomAnim = false)
	{
		MovedElement.Moving = true;

		Vector2I OldLocation = (Vector2I)MovedElement.Location;
		Vector2I NewLocation = GetTargetLocation(MovedElement, MovementDirection, Step);

		int SavedStepCount = StepCount + 1;
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

		MoveElementByLocation(MovedElement, NewLocation, UseZoomAnim);
	}

	private void MoveElementByLocation(Element MovedElement, Vector2I NewLocation, bool UseZoomAnim = false)
	{
		Vector2 NewPosition = new Vector2(NewLocation.Y * 64 + 32, NewLocation.X * 64 + 32);
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

		if (UseZoomAnim)
		{
			PackedScene NobleSnailGhostScene = (PackedScene)GD.Load("res://Scenes/Game/NobleSnailGhost.tscn");
			Node2D MyNobleSnailGhost = (Node2D)NobleSnailGhostScene.Instantiate();
			MyNobleSnailGhost.Position = new Vector2(OldLocation.Y * 64 + 32, OldLocation.X * 64 + 32);
			AddChild(MyNobleSnailGhost);
			MovedElement.Position = NewPosition;
			MovedElement.GetNode<AnimationPlayer>("AnimationPlayer").Play("ZoomIn");
		}
		else
		{
			CreateTween()
			.TweenProperty(MovedElement, "position", NewPosition, 0.2f)
			.SetEase(Tween.EaseType.Out)
			.Connect("finished", Callable.From(() => ResetElementMoving(MovedElement)));
		}
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

		Element FacingElement = GetFacingElement(CheckedElement, MovementDirection);
		if (FacingElement == null)
		{
			return true;
		}

		if (FacingElement.Type == ElementType.Barrier)
		{
			return false;
		}
		if (FacingElement.Type == ElementType.Player)
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
		if (CheckedSnail.CanMove == false)
		{
			return false;
		}

		Element FacingElement = GetFacingElement(CheckedSnail, MovementDirection);
		if (FacingElement != null && FacingElement.Type == ElementType.Snail)
		{
			Snail FacingSnail = (Snail)FacingElement;
			if (CheckedSnail.Kind == SnailKind.Dark)
			{
				FacingSnail.RemovedBy = SnailKind.Fire;
				CheckedSnail.OnMove(this, MovementDirection);
				return true;
			}
			else if (CheckedSnail.Kind == SnailKind.Noble)
			{
				return HandleNobleSnail((NobleSnail)CheckedSnail, MovementDirection);
			}

			if (FacingSnail.Kind == SnailKind.Fire)
			{
				if (CheckedSnail.Kind == SnailKind.Leaf)
				{
					LeafSnail MyLeafSnail = (LeafSnail)CheckedSnail;
					MyLeafSnail.RemovedBy = SnailKind.Fire;
					MyLeafSnail.EnterFireSnail(this, (FireSnail)FacingElement);
					return true;
				}
				else if (CheckedSnail.Kind == SnailKind.Water)
				{
					WaterSnail MyWaterSnail = (WaterSnail)CheckedSnail;
					MyWaterSnail.RemovedBy = SnailKind.Fire;
					MyWaterSnail.EnterFireSnail(this, (FireSnail)FacingElement);
					return true;
				}

				return false;
			}

			return false;
		}
		else if (FacingElement != null && FacingElement.Type == ElementType.Door)
		{
			if (CheckedSnail.Kind == SnailKind.Noble)
			{
				return HandleNobleSnail((NobleSnail)CheckedSnail, MovementDirection);
			}

			return false;
		}
		else if (FacingElement != null && FacingElement.Type == ElementType.Barrier)
		{
			if (CheckedSnail.Kind == SnailKind.Noble)
			{
				return HandleNobleSnail((NobleSnail)CheckedSnail, MovementDirection);
			}

			return false;
		}
		else if (FacingElement != null && FacingElement.Type == ElementType.Player)
		{
			return false;
		}
		else if (FacingElement != null && FacingElement.Type == ElementType.TargetPoint)
		{
			if (CheckedSnail.Kind == SnailKind.Noble)
			{
				return HandleNobleSnail((NobleSnail)CheckedSnail, MovementDirection);
			}
			else if (CheckedSnail.Kind == SnailKind.Metal)
			{
				return HandleMetalSnail((MetalSnail)CheckedSnail, MovementDirection);
			}

			TargetPoint MyTargetPoint = (TargetPoint)FacingElement;
			if (CheckedSnail.Kind == SnailKind.Rainbow)
			{
				MoveElement(CheckedSnail, MovementDirection);
				return true;
			}
			
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

		if (CheckedSnail.Kind == SnailKind.Metal)
		{
			return HandleMetalSnail((MetalSnail)CheckedSnail, MovementDirection);
		}

		if (IsElementCanMove(CheckedSnail, MovementDirection))
		{
			MoveElement(CheckedSnail, MovementDirection);
			CheckedSnail.OnMove(this, MovementDirection);
			return true;
		}

		return false;
	}

	private bool HandleNobleSnail(NobleSnail InNobleSnail, Direction MovementDirection)
	{
		if (InNobleSnail.TryGetTeleportStep(this, MovementDirection, out int Step))
		{
			if (Step == 1)
			{
				MoveElement(InNobleSnail, MovementDirection, Step);
			}
			else
			{
				MoveElement(InNobleSnail, MovementDirection, Step, true);
			}
			
			return true;
		}

		return false;
	}

	private bool HandleMetalSnail(MetalSnail InMetalSnail, Direction MovementDirection)
	{
		if (InMetalSnail.TryGetTargetStep(this, MovementDirection, out int Step))
		{
			MoveElement(InMetalSnail, MovementDirection, Step);
			return true;
		}
		else
		{
			return false;
		}
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
		if (MapId == 16)
		{
			PackedScene ThanksScene = (PackedScene)GD.Load("res://Scenes/UI/Thanks.tscn");
			Thanks MyThanks = (Thanks)ThanksScene.Instantiate();
			AddChild(MyThanks);
			return false;
		}

		if (CheckedDoor.Accept == true)
		{
			MySignals.EmitSignal("LevelStarted", MapBean.NextLevel);
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
		MoveAllPlayer(Direction.Up);
	}

	private void DownKeyDown()
	{
		MoveAllPlayer(Direction.Down);
	}

	private void LeftKeyDown()
	{
		MoveAllPlayer(Direction.Left);
	}

	private void RightKeyDown()
	{
		MoveAllPlayer(Direction.Right);
	}

	private void MoveAllPlayer(Direction MovementDirection)
	{
		bool PlayerMoved = false;
		for (int i = 0; i < MyPlayers.Count; i++)
		{
			Player MyPlayer = MyPlayers[i];
			if (MyPlayer.CanMove == false)
			{
				continue;
			}

			if (MyPlayer.Moving || HandleFacingElement(MyPlayer, MovementDirection) == false)
			{
				continue;
			}

			PlayerMoved = true;
			Element FacingElement = GetFacingElement(MyPlayer, MovementDirection);
			if (FacingElement is DarkSnail && GetFacingElement(FacingElement, MovementDirection) != null)
			{
				MoveElement(MyPlayer, MovementDirection, 0);
			}
			else
			{
				MoveElement(MyPlayer, MovementDirection);
			}
		}

		if (PlayerMoved)
		{
			StepCount += 1;
			StepCountLabel.Text = StepCount.ToString();
		}
	}

	private void ResetElementMoving(Element MovedElement)
	{
		MovedElement.Moving = false;
	}

	private void SpaceKeyDown()
	{
		if (CanRedo == false || ElementLocationHistory.Count == 0)
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

			if (Key is NobleSnail && IsStepGreaterOne(Key.Location, OldLocation))
			{
				MoveElementByLocation(Key, OldLocation, true);
			}
			else
			{
				MoveElementByLocation(Key, OldLocation);
			}
			
			if (Key is Snail)
			{
				Snail RedoSnail = Key as Snail;
				if (RedoSnail != null)
				{
					RedoSnail.OnRedo(this, OldLocation);
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

	private void Restart()
	{
		MySignals.EmitSignal("LevelStarted", MapId);
	}

	private bool IsStepGreaterOne(Vector2I OldLocation, Vector2I NewLocation)
	{
		int X1 = OldLocation.X;
		int Y1 = OldLocation.Y;
		int X2 = NewLocation.X;
		int Y2 = NewLocation.Y;

		if (Mathf.Abs(X1 - X2) > 1 || Mathf.Abs(Y1 - Y2) > 1)
		{
			return true;
		}

		return false;
	}

	public void RemoveElement(Element RemovedElement)
	{
		RemovedElement.CanMove = false;
		foreach (Player MyPlayer in MyPlayers)
		{
			MyPlayer.CanMove = false;
		}

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

	public Queue<Element> RemovalElementQueue = new Queue<Element>();
	public void RemoveElementCallable(string AnimName)
	{
		List<Vector2> ExplosionPositions = new List<Vector2>();
		foreach (Element RemovedElement in RemovalElementQueue)
		{
			RemovedElement.CanMove = true;
			RemoveChild(RemovedElement);
			MapMatrix[RemovedElement.Location.X, RemovedElement.Location.Y] = null;
			
			if (RemovedElement is WaterSnail || RemovedElement is LeafSnail)
			{
				if (RemovedElement.RemovedBy == SnailKind.Fire)
				{
					continue;
				}
			}

			ExplosionPositions.Add(RemovedElement.Position);
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
				foreach (Player MyPlayer in MyPlayers)
				{
					MyPlayer.CanMove = true;
					CanRedo = true;
				}
			};
		}
	}
}
