using Godot;
using System;

public partial class Player : Element
{
	private ControlSignals MyControlSignals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MyControlSignals = GetNode<ControlSignals>("/root/ControlSignals");
		MyControlSignals.Up += MoveUp;
		MyControlSignals.Down += MoveDown;
		MyControlSignals.Left += MoveLeft;
		MyControlSignals.Right += MoveRight;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void MoveUp()
	{
		Position = new Vector2(Position.X, Position.Y - 2);
	}

	private void MoveDown()
	{
		Position = new Vector2(Position.X, Position.Y + 2);
	}

	private void MoveLeft()
	{
		Position = new Vector2(Position.X - 2, Position.Y);
	}

	private void MoveRight()
	{
		Position = new Vector2(Position.X + 2, Position.Y);
	}
}
