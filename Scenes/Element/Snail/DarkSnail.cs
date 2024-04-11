using Godot;
using System;

public partial class DarkSnail : Snail
{
    public override void OnMove(Level InLevel, Direction MovementDirection)
	{
		Element FacingElement = InLevel.GetFacingElement(this, MovementDirection);
		if (FacingElement != null && FacingElement.Type == ElementType.Snail)
		{
			CanMove = false;
			InLevel.CanRedo = false;
			InLevel.RemoveElement(FacingElement);
			InLevel.RemoveElement(this);
		}
	}

	public override void OnRedo(Level InLevel, Vector2I OldLocation)
	{
		CanMove = true;
	}
}
