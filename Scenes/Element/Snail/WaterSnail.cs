using Godot;
using System;

public partial class WaterSnail : Snail
{
	FireSnail TargetFireSnail;
	
	public void EnterFireSnail(Level InLevel, FireSnail MyFireSnail)
	{
		TargetFireSnail = MyFireSnail;
		MyFireSnail.MinusOne(InLevel);

		CreateTween()
		.TweenProperty(this, "position", MyFireSnail.Position, 0.2f)
		.SetEase(Tween.EaseType.Out)
		.Connect("finished", Callable.From(() => InLevel.RemoveChild(this)));

		if (InLevel.ElementLocationHistory.TryGetValue(InLevel.StepCount + 1, out FOneStep OneStep))
		{
			OneStep.RemovedElements.Add(this);
			OneStep.MovedElements.Add(this, Location);
		}
		else
		{
			FOneStep NewOneStep = new FOneStep();
			NewOneStep.RemovedElements.Add(this);
			NewOneStep.MovedElements.Add(this, Location);
			InLevel.ElementLocationHistory.Add(InLevel.StepCount + 1, NewOneStep);
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
