using Godot;
using System;

public partial class Snail : Element
{
	[Export]
	public SnailKind Kind = SnailKind.None;
	public bool InTargetPoint = false;

	public virtual void OnMove(Level InLevel, Direction MovementDirection)
	{
		
	}

	public virtual void OnRedo(Level InLevel, Vector2I OldLocation)
	{
		
	}
}
