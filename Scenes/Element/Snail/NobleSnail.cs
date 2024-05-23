using Godot;
using System;

public partial class NobleSnail : Snail
{
	public bool TryGetTeleportStep(Level InLevel, Direction MovementDirection, out int Step)
	{
		Vector2I Vec = new Vector2I();
		switch (MovementDirection)
		{
			case Direction.Up: 		Vec = new Vector2I(-1, 0); break;
			case Direction.Down:	Vec = new Vector2I(1, 0); break;
			case Direction.Left:	Vec = new Vector2I(0, -1); break;
			case Direction.Right:	Vec = new Vector2I(0, 1); break;
		}

		Vector2I TargetLocation = this.Location + Vec;
		bool Found = false;
		Step = 1;
		Player FarthestPlayer = null;
		// Useful when its a player on the current forward path
		int SpecialStep = 0;

		while (TargetLocation.X >= 0 && TargetLocation.X < 8 && TargetLocation.Y >= 0 && TargetLocation.Y < 8)
		{
			Element CheckedElement = InLevel.MapMatrix[TargetLocation.X, TargetLocation.Y];
			if (CheckedElement == null || (CheckedElement is TargetPoint && ((TargetPoint)CheckedElement).Kind == SnailKind.Noble))
			{
				Found = true;
				break;
			}

			if (CheckedElement is Player)
			{
				FarthestPlayer = (Player)CheckedElement;
				SpecialStep = Step;
			}

			Step += 1;
			TargetLocation += Vec;
		}

		if (FarthestPlayer != null)
		{
			if (LevelRef.IsElementCanMove(FarthestPlayer, MovementDirection))
			{
				Step = SpecialStep;
				LevelRef.MoveElement(FarthestPlayer, MovementDirection);
			}
		}

		return Found;
	}
}
