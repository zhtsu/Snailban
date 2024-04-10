using Godot;
using System;

public partial class Snail : Element
{
	[Export]
	public SnailKind Kind = SnailKind.None;

	public virtual bool HandleMove(Level InLevel, Direction MovementDirection)
	{
		return false;
	}

	public virtual void HandleRedo(Level InLevel, Vector2I OldLocation)
	{
		
	}
}
