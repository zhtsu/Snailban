using Godot;
using System;

public partial class Explosion : Node2D
{
	[Signal]
	public delegate void ExplosionFinishedEventHandler();
	private AnimatedSprite2D AnimSprite2D;

	public override void _Ready()
	{
		AnimSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AnimSprite2D.Play();
		AnimSprite2D.Connect("animation_finished", Callable.From(() => {
			EmitSignal("ExplosionFinished");
			QueueFree();
		}));
	}
}
