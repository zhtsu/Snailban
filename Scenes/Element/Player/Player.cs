using Godot;
using System;
using System.Numerics;

public partial class Player : Element
{
	private AnimationPlayer AnimPlayer;
	private Timer BlinkTimer;
	private bool FirstBlink = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		Type = ElementType.Player;

		AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		BlinkTimer = GetNode<Timer>("Timer");

		BlinkTimer.Connect("timeout", new Callable(this, nameof(StartBlinkTimer)));
		StartBlinkTimer();
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
