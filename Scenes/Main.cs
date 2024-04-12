using Godot;
using System;

public partial class Main : Node
{
	private CustomSignals MySignals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MySignals = GetNode<CustomSignals>("/root/CustomSignals");

		MySignals.LevelStarted += LoadLevel;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
			else if (Input.IsActionJustPressed("ESC"))
			{
				foreach (Node Child in GetChildren())
				{
					RemoveChild(Child);
					Child.QueueFree();
				}

				PackedScene MainMenuSence = (PackedScene)GD.Load("res://Scenes/UI/MainMenu.tscn");
				MainMenu MyMainMenu = (MainMenu)MainMenuSence.Instantiate();
				AddChild(MyMainMenu);
			}
		}
    }

	private void LoadLevel(int MapId)
	{
		foreach (Node Child in GetChildren())
		{
			RemoveChild(Child);
			Child.QueueFree();
		}

		PackedScene LevelScene = (PackedScene)GD.Load("res://Scenes/Game/Level.tscn");
		Level MyLevel = (Level)LevelScene.Instantiate();
		MyLevel.MapId = MapId;
		AddChild(MyLevel);
	}
}
