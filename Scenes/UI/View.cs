using Godot;
using System;
using System.Collections;

public partial class View : CanvasLayer
{
	private CustomSignals MySignals;
	public int LastLevel = 1;
	public int MaxLevel = 1;
	private TextureRect Cursor;
	public int SelectedLevel = 1;

	public void Init(int InLastLevel, int InMaxLevel)
	{
		LastLevel = InLastLevel;
		MaxLevel = InMaxLevel;
	}

	public override void _Ready()
	{
		MySignals = GetNode<CustomSignals>("/root/CustomSignals");
		Cursor = GetNode<TextureRect>("Cursor");

		MySignals.SpaceKey += SpaceKeyDown;
		MySignals.LeftKey += LeftKeyDown;
		MySignals.RightKey += RightKeyDown;

		SelectedLevel = LastLevel;
		
		Texture2D LevelLockerTexture = (Texture2D)GD.Load("res://Assets/Textures/level_locker.png");

		for (int i = 0; i < 16; i++)
		{
			TextureRect LevelLocker = new TextureRect();
			if (i + 1 > MaxLevel)
			{
				LevelLocker.Texture = LevelLockerTexture;
			}
			else if (i + 1 == LastLevel)
			{
				SetCursorPosition(i);
			}

			LevelLocker.Position = new Vector2((i % 4) * 128, (i / 4) * 128);
			AddChild(LevelLocker);
		}
	}

    public override void _ExitTree()
    {
		MySignals.SpaceKey -= SpaceKeyDown;
		MySignals.LeftKey -= LeftKeyDown;
		MySignals.RightKey -= RightKeyDown;
    }

	private void SpaceKeyDown()
	{
		MySignals.EmitSignal("LevelStarted", SelectedLevel);
	}

	private void LeftKeyDown()
	{
		if (SelectedLevel > 1)
		{
			SelectedLevel -= 1;
			SetCursorPosition(SelectedLevel - 1);
		}
	}

	private void RightKeyDown()
	{
		if (SelectedLevel < 16 && SelectedLevel < MaxLevel)
		{
			SelectedLevel += 1;
			SetCursorPosition(SelectedLevel - 1);
		}
	}

	private void SetCursorPosition(int Level)
	{
		Cursor.Position = new Vector2((Level % 4) * 128 + 81, (Level / 4) * 128 + 81);
	}
}
