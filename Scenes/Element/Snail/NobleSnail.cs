using Godot;
using System;

public partial class NobleSnail : Snail
{
	public bool GetTeleportStep(Level InLevel, Direction MovementDirection, out int Step)
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
		while (TargetLocation.X > 0 && TargetLocation.X < 8 && TargetLocation.Y > 0 && TargetLocation.Y < 8)
		{
			if (InLevel.MapMatrix[TargetLocation.X, TargetLocation.Y] == null)
			{
				Found = true;
				break;
			}

			Step += 1;
			TargetLocation += Vec;
		}

		return Found;
	}
}
