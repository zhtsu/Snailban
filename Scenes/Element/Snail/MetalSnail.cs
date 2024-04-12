using Godot;
using System;

public partial class MetalSnail : Snail
{
	public bool TryGetTargetStep(Level InLevel, Direction MovementDirection, out int Step)
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
		Step = 0;
		while (TargetLocation.X >= 0 && TargetLocation.X < 8 && TargetLocation.Y >= 0 && TargetLocation.Y < 8)
		{
			Element CheckedElement = InLevel.MapMatrix[TargetLocation.X, TargetLocation.Y];
			if (CheckedElement == null || (CheckedElement is TargetPoint && ((TargetPoint)CheckedElement).Kind == SnailKind.Metal))
			{
				Step += 1;
				TargetLocation += Vec;
			}
			else
			{
				break;
			}
		}

		if (Step == 0)
		{
			return false;
		}

		return true;
	}
}
