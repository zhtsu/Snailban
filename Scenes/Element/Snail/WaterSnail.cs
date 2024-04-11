using Godot;
using System;

public partial class WaterSnail : Snail
{
	FireSnail TargetFireSnail;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void OnMove(Level InLevel, Direction MovementDirection)
	{
		Element FacingElement = InLevel.GetFacingElement(this, MovementDirection);
		if (FacingElement != null && FacingElement.Type == ElementType.Snail)
		{
			Snail MySnail = (Snail)FacingElement;
			if (MySnail != null && MySnail.Kind == SnailKind.Fire)
			{
				FireSnail MyFireSnail = (FireSnail)MySnail;
				if (MyFireSnail != null)
				{
					int SavedStepCount = InLevel.StepCount + 1;

					if (InLevel.ElementLocationHistory.TryGetValue(SavedStepCount, out FOneStep OneStep))
					{
						OneStep.MovedElements.Add(this, Location);
						OneStep.RemovedElements.Add(this);
					}
					else
					{
						FOneStep NewOneStep = new FOneStep();
						NewOneStep.MovedElements.Add(this, Location);
						NewOneStep.RemovedElements.Add(this);
						InLevel.ElementLocationHistory.Add(SavedStepCount, NewOneStep);
					}

					TargetFireSnail = MyFireSnail;
					MyFireSnail.MinusOne(InLevel);
					InLevel.MapMatrix[Location.X, Location.Y] = null;
					CreateTween()
					.TweenProperty(this, "position", MyFireSnail.Position, 0.2f)
					.SetEase(Tween.EaseType.Out)
					.Connect("finished", Callable.From(() => InLevel.RemoveChild(this)));
				}
			}
		}
	}

	public override void OnRedo(Level InLevel, Vector2I OldLocation)
	{
		if (TargetFireSnail != null && TargetFireSnail.Countdown < 3)
		{
			TargetFireSnail.PlusOne();
		}
	}
}
