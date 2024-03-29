using Godot;
using System;

public partial class MainMenu : CanvasLayer
{
	private CustomSignals MySignals;
	private int CursorIndex = 0;
	private float[] CursorYArray = { 336, 406 };
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

		CreateTween().TweenProperty(Cursor, "position", new Vector2(32, CursorYArray[CursorIndex]), 0.4f);
	}

	private void DownKeyDown()
	{
		if (CursorIndex == 1)
		{
			return;
		}

		CursorIndex += 1;
		
		CreateTween().TweenProperty(Cursor, "position", new Vector2(32, CursorYArray[CursorIndex]), 0.4f);
	}

	private void SpaceKeyDown()
	{
		if (CursorIndex == 0)
		{
			StartGame();
		}
		else if (CursorIndex == 1)
		{
			SwitchLanguage();
		}
	}

	private void StartGame()
	{
		MySignals.EmitSignal("LevelStarted", 0);
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
