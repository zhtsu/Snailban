using Godot;
using System;

public partial class TargetPoint : Element
{
	[Export]
	public SnailKind Kind = SnailKind.None;
	public bool Completed = false;
}
