using Godot;
using System;

public partial class Player : Element
{
	private MySignals _MySignals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_MySignals = GetNode<MySignals>("/root/MySignals");
		_MySignals.UpKey += MoveUp;
		_MySignals.DownKey += MoveDown;
		_MySignals.LeftKey += MoveLeft;
		_MySignals.RightKey += MoveRight;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void MoveUp()
	{
		
	}

	private void MoveDown()
	{
		
	}

	private void MoveLeft()
	{
		
	}

	private void MoveRight()
	{
		
	}
}
