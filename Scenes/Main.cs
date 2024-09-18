using Godot;
using System;

public partial class Main : Node
{
	private CustomSignals MySignals;

	private SubViewport GameViewport;
	private Button UpButton, DownButton, LeftButton, RightButton, SpaceButton, RButton, ESCButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MySignals = GetNode<CustomSignals>("/root/CustomSignals");

		UpButton = GetNode<Button>("Panel/Panel1/Panel/UpButton");
		UpButton.ButtonDown += () => MySignals.EmitSignal("UpKey");

		DownButton = GetNode<Button>("Panel/Panel1/Panel3/DownButton");
		DownButton.ButtonDown += () => MySignals.EmitSignal("DownKey");

		LeftButton = GetNode<Button>("Panel/Panel1/Panel4/LeftButton");
		LeftButton.ButtonDown += () => MySignals.EmitSignal("LeftKey");

		RightButton = GetNode<Button>("Panel/Panel1/Panel5/RightButton");
		RightButton.ButtonDown += () => MySignals.EmitSignal("RightKey");

		SpaceButton = GetNode<Button>("Panel/Panel3/Panel/SpaceButton");
		SpaceButton.ButtonDown += () => MySignals.EmitSignal("SpaceKey");

		RButton = GetNode<Button>("Panel/Panel3/Panel2/RButton");
		RButton.ButtonDown += () => MySignals.EmitSignal("Restart");

		ESCButton = GetNode<Button>("Panel/Panel3/Panel3/ESCButton");
		ESCButton.ButtonDown += () => BackToMainMenu();

		GameViewport = GetNode<SubViewport>("SubViewport/SubViewport");

		MySignals.LevelStarted += LoadLevel;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event is InputEventKey EventKey)
		{
			if (Input.IsActionJustPressed("Up"))
			{
				MySignals.EmitSignal("UpKey");
			}
			else if (Input.IsActionJustPressed("Down"))
			{
				MySignals.EmitSignal("DownKey");
			}
			else if (Input.IsActionJustPressed("Left"))
			{
				MySignals.EmitSignal("LeftKey");
			}
			else if (Input.IsActionJustPressed("Right"))
			{
				MySignals.EmitSignal("RightKey");
			}
			else if (Input.IsActionJustPressed("Space"))
			{
				MySignals.EmitSignal("SpaceKey");
			}
			else if (Input.IsActionJustPressed("R"))
			{
				MySignals.EmitSignal("Restart");
			}
			else if (Input.IsActionJustPressed("ESC"))
			{
				BackToMainMenu();
			}
		}
	}

	private void LoadLevel(int MapId)
	{
		foreach (Node Child in GameViewport.GetChildren())
		{
			GameViewport.RemoveChild(Child);
			Child.QueueFree();
		}

		PackedScene LevelScene = (PackedScene)GD.Load("res://Scenes/Game/Level.tscn");
		Level MyLevel = (Level)LevelScene.Instantiate();
		MyLevel.MapId = MapId;
		GameViewport.AddChild(MyLevel);
	}

	public void BackToMainMenu()
	{
		foreach (Node Child in GameViewport.GetChildren())
		{
			GameViewport.RemoveChild(Child);
			Child.QueueFree();
		}

		PackedScene MainMenuSence = (PackedScene)GD.Load("res://Scenes/UI/MainMenu.tscn");
		MainMenu MyMainMenu = (MainMenu)MainMenuSence.Instantiate();
		GameViewport.AddChild(MyMainMenu);
	}
}
