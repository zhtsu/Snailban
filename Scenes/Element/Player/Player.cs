using Godot;
using System;
using System.Numerics;

public partial class Player : Element
{
	private AnimationPlayer AnimPlayer;
	private Timer BlinkTimer;
	private bool FirstBlink = true;
	public System.Numerics.Vector2 Location = new System.Numerics.Vector2(0, 0);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		BlinkTimer = GetNode<Timer>("Timer");

		

		BlinkTimer.Connect("timeout", new Callable(this, nameof(StartBlinkTimer)));
		StartBlinkTimer();
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

	private void StartBlinkTimer()
	{
		Random Rd = new Random();
		BlinkTimer.Start(Rd.NextDouble() * 52 + 8);

		if (FirstBlink)
		{
			FirstBlink = false;
			return;
		}

		AnimPlayer.Play("Blink");
	}
}
