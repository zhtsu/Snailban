using Godot;
using System;

public partial class Snail : Element
{
	[Export]
	public SnailKind Kind = SnailKind.None;

	public virtual void HandleMove(Level InLevel, Direction MovementDirection)
	{

	}

	public virtual void HandleRedo(Level InLevel, Vector2I OldLocation)
	{

	}
}
