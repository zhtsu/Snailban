using Godot;
using System;

public partial class FireSnail : Snail
{
	private Label CountdownLabel;
	public int Countdown = 3;

    public override void _Ready()
    {
		CountdownLabel = GetNode<Label>("CountdownLabel");
    }

    public override void OnMove(Level InLevel, Direction MovementDirection)
	{
		Countdown -= 1;
		CountdownLabel.Text = Countdown.ToString();
		if (Countdown == 0)
		{
			CanMove = false;
			InLevel.CanRedo = false;
			Element FacingElement = InLevel.GetFacingElement(this, MovementDirection);
			if (FacingElement != null && FacingElement.Type == ElementType.Barrier)
			{
				InLevel.RemoveElement(FacingElement);
			}
			InLevel.RemoveElement(this);
		}
	}

	public override void OnRedo(Level InLevel, Vector2I OldLocation)
	{
		if (Countdown == 0)
		{
			CanMove = true;
		}

		Countdown += 1;
		CountdownLabel.Text = Countdown.ToString();
	}

	// For LeafSnail
	public void PlusOne()
	{
		Countdown += 1;
		CountdownLabel.Text = Countdown.ToString();
	}

	// For WaterSnail
	public void MinusOne(Level InLevel)
	{
		Countdown -= 1;
		CountdownLabel.Text = Countdown.ToString();
		if (Countdown == 0)
		{
			InLevel.RemoveElement(this);
		}
	}
}
