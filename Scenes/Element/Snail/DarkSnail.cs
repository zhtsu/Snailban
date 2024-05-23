using Godot;
using System;

public partial class DarkSnail : Snail
{
    public override void OnMove(Level InLevel, Direction MovementDirection)
	{
		Element FacingElement = InLevel.GetFacingElement(this, MovementDirection);
		if (FacingElement != null && FacingElement.Type == ElementType.Snail)
		{
			if (((Snail)FacingElement).InTargetPoint)
			{
				return;
			}

			CanMove = false;
			foreach (Player MyPlayer in InLevel.MyPlayers)
			{
				MyPlayer.CanMove = false;
			}

			InLevel.CanRedo = false;
			InLevel.RemoveElement(FacingElement);
			InLevel.RemoveElement(this);
		}
	}
}
