using Godot;
using System;
using System.Collections.Generic;

public partial class MainMenu : CanvasLayer
{
	private CustomSignals MySignals;
	private int CursorIndex = 0;
	private float[] CursorYArray = { 216, 286, 354 };
	private TextureRect Cursor;
	private Label LanguageLable;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MySignals = GetNode<CustomSignals>("/root/CustomSignals");
		Cursor = GetNode<TextureRect>("Cursor");
		LanguageLable = GetNode<Label>("LanguageLabel");

		MySignals.UpKey += UpKeyDown;
		MySignals.DownKey += DownKeyDown;
		MySignals.SpaceKey += SpaceKeyDown;

		Texture2D CursorTexture = (Texture2D)GD.Load(ConfigData.SnailTexturePaths.PickRandom());
		Cursor.Texture = CursorTexture;
	}

    public override void _ExitTree()
    {
        base._ExitTree();

		MySignals.UpKey -= UpKeyDown;
		MySignals.DownKey -= DownKeyDown;
		MySignals.SpaceKey -= SpaceKeyDown;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	private void UpKeyDown()
	{
		if (CursorIndex == 0)
		{
			return;
		}

		CursorIndex -= 1;

		CreateTween()
		.TweenProperty(Cursor, "position", new Vector2(96, CursorYArray[CursorIndex]), 0.3f)
		.SetEase(Tween.EaseType.Out);
	}

	private void DownKeyDown()
	{
		if (CursorIndex == 2)
		{
			return;
		}

		CursorIndex += 1;
		
		CreateTween()
		.TweenProperty(Cursor, "position", new Vector2(96, CursorYArray[CursorIndex]), 0.3f)
		.SetEase(Tween.EaseType.Out);
	}

	private void SpaceKeyDown()
	{
		if (CursorIndex == 0)
		{
			NewGame();
		}
		else if (CursorIndex == 1)
		{
			LastMaxLevel();
		}
		else if (CursorIndex == 2)
		{
			SwitchLanguage();
		}
	}

	private void NewGame()
	{
		MySignals.EmitSignal("LevelStarted", 0);
	}

	private void LastMaxLevel()
	{

	}

	private void SwitchLanguage()
	{
		if (LanguageLable.Text == "EN")
		{
			LanguageLable.Text = "ZH";
		}
		else if (LanguageLable.Text == "ZH")
		{
			LanguageLable.Text = "EN";
		}
	}
}
