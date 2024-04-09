using Godot;
using System;

public partial class FireSnail : Snail
{
	private Label CountdownLabel;
	private AnimationPlayer AnimPlayer;
	private int Countdown = 3;

    public override void _Ready()
    {
		CountdownLabel = GetNode<Label>("CountdownLabel");
		AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void HandleMove(Level InLevel, Direction MovementDirection)
	{
		Countdown -= 1;
		CountdownLabel.Text = Countdown.ToString();
		if (Countdown == 0)
		{
			CanMove = false;
			Element FacingElement = InLevel.GetFacingElement(this, MovementDirection);
			if (FacingElement != null && FacingElement.Type == ElementType.Barrier)
			{
				InLevel.RemoveElement(FacingElement);
			}
			InLevel.RemoveElement(this);
		}
	}

	public override void HandleRedo(Level InLevel, Vector2I OldLocation)
	{
		if (Countdown == 0)
		{
			CanMove = true;
		}

		Countdown += 1;
		CountdownLabel.Text = Countdown.ToString();
	}
}
