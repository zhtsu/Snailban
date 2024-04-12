using Godot;
using System;
using System.Collections;

public partial class View : CanvasLayer
{
	[Signal]
    public delegate void ClosedEventHandler();

	private CustomSignals MySignals;
	public int LastLevel = 1;

	public override void _Ready()
	{
		MySignals = GetNode<CustomSignals>("/root/CustomSignals");

		MySignals.SpaceKey += SpaceKeyDown;
		
		Texture2D LevelLockerTexture = (Texture2D)GD.Load("res://Assets/Textures/level_locker.png");
		for (int i = 0; i < 16; i++)
		{
			TextureRect LevelLocker = new TextureRect();
			if (i + 1 > LastLevel)
			{
				LevelLocker.Texture = LevelLockerTexture;
			}
			LevelLocker.Position = new Vector2((i % 4) * 128, (i / 4) * 128);
			AddChild(LevelLocker);
		}
	}

    public override void _ExitTree()
    {
		MySignals.SpaceKey -= SpaceKeyDown;
    }

	private void SpaceKeyDown()
	{
		EmitSignal("Closed");
		QueueFree();
	}
}
