using Godot;
using System;
using System.Collections.Generic;

public partial class MainMenu : CanvasLayer
{
	private CustomSignals MySignals;
	private int CursorIndex = 0;
	private float[] CursorYArray = { 216, 286, 354 };
	private int LastLevel = 1;
	private int MaxLevel = 1;
	private TextureRect Cursor;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MySignals = GetNode<CustomSignals>("/root/CustomSignals");
		Cursor = GetNode<TextureRect>("Cursor");

		MySignals.UpKey += UpKeyDown;
		MySignals.DownKey += DownKeyDown;
		MySignals.SpaceKey += SpaceKeyDown;

		Texture2D CursorTexture = (Texture2D)GD.Load(ConfigData.SnailTexturePaths.PickRandom());
		Cursor.Texture = CursorTexture;

		if (FileAccess.FileExists(ConfigData.user_data_path))
		{
			Godot.Collections.Dictionary UserData = MyMethods.LoadJson(ConfigData.user_data_path);
			LastLevel = (int)UserData["last_level"];
			MaxLevel = (int)UserData["max_level"];
		}
		else
		{
			FileAccess UserDataFile = FileAccess.Open(ConfigData.user_data_path, FileAccess.ModeFlags.Write);
			Godot.Collections.Dictionary UserData = new Godot.Collections.Dictionary();
			UserData["last_level"] = 1;
			UserData["max_level"] = 1;
			UserDataFile.StoreString(UserData.ToString());
			UserDataFile.Close();
		}
	}

    public override void _ExitTree()
    {
        base._ExitTree();

		MySignals.UpKey -= UpKeyDown;
		MySignals.DownKey -= DownKeyDown;
		MySignals.SpaceKey -= SpaceKeyDown;
    }

	private void UpKeyDown()
	{
		if (CursorIndex == 0)
		{
			return;
		}

		CursorIndex -= 1;

		CreateTween()
		.TweenProperty(Cursor, "position", new Vector2(64, CursorYArray[CursorIndex]), 0.3f)
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
		.TweenProperty(Cursor, "position", new Vector2(64, CursorYArray[CursorIndex]), 0.3f)
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
			StartLastLevel();
		}
		else if (CursorIndex == 2)
		{
			OpenLevelView();
		}
	}

	private void NewGame()
	{
		MySignals.EmitSignal("LevelStarted", 1);

		Godot.Collections.Dictionary UserData = new Godot.Collections.Dictionary();
		UserData["last_level"] = 1;
		UserData["max_level"] = 1;
		
		FileAccess UserDataFile = FileAccess.Open(ConfigData.user_data_path, FileAccess.ModeFlags.Write);
		UserDataFile.StoreString(UserData.ToString());
		UserDataFile.Close();
	}

	private void StartLastLevel()
	{
		MySignals.EmitSignal("LevelStarted", LastLevel);
	}

	private void OpenLevelView()
	{
		PackedScene ViewScene = (PackedScene)GD.Load("res://Scenes/UI/View.tscn");
		View MyView = (View)ViewScene.Instantiate();
		MyView.Init(LastLevel, MaxLevel);
		AddChild(MyView);
	}
}
