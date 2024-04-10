using Godot;
using System;
using System.Numerics;

public partial class Player : Element
{
	private AnimationPlayer BlinkAnimPlayer;
	private Timer BlinkTimer;
	private bool FirstBlink = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		Type = ElementType.Player;

		BlinkAnimPlayer = GetNode<AnimationPlayer>("BlinkAnimPlayer");
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

		BlinkAnimPlayer.Play("Blink");
	}
}
