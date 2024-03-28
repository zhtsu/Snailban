using Godot;
using System;

public partial class MainMenu : CanvasLayer
{
	private CustomSignals MySignals;
	private int CursorIndex = 0;
	private float[] CursorYArray = { 336, 406 };
	private TextureRect Cursor;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MySignals = GetNode<CustomSignals>("/root/CustomSignals");
		Cursor = GetNode<TextureRect>("Cursor");

		MySignals.UpKey += UpKeyDown;
		MySignals.DownKey += DownKeyDown;
		MySignals.SpaceKey += SpaceKeyDown;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpKeyDown()
	{
		if (CursorIndex == 0)
		{
			return;
		}

		CursorIndex -= 1;

		CreateTween().TweenProperty(Cursor, "position", new Vector2(32, CursorYArray[CursorIndex]), 0.4f);
	}

	public void DownKeyDown()
	{
		if (CursorIndex == 1)
		{
			return;
		}

		CursorIndex += 1;
		
		CreateTween().TweenProperty(Cursor, "position", new Vector2(32, CursorYArray[CursorIndex]), 0.4f);
	}

	public void SpaceKeyDown()
	{

	}
}
