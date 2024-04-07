using Godot;
using System;

public partial class TargetPoint : Element
{
	[Signal]
    public delegate void SnailEnteredEventHandler();

	[Signal]
    public delegate void SnailExitedEventHandler();

	[Export]
	public SnailKind Kind = SnailKind.None;
	public bool Completed = false;

	private Area2D Area;

    public override void _Ready()
    {
		Area = GetNode<Area2D>("Area2D");
		Area.Connect("area_entered", new Callable(this, nameof(OnSnailEntered)));
		Area.Connect("area_exited", new Callable(this, nameof(OnSnailExited)));
    }

	private void OnSnailEntered(Area2D EnteredArea)
	{
		if (EnteredArea.Owner is Snail)
		{
			Completed = true;
			Snail EnteredSnail = EnteredArea.Owner as Snail;
			if (EnteredSnail != null)
			{
				EnteredSnail.CanMove = false;
				GD.Print("Enter");
			}
			EmitSignal("SnailEntered");
		}
	}

	private void OnSnailExited(Area2D ExitedArea)
	{
		if (ExitedArea.Owner is Snail)
		{
			Completed = false;
			Snail EnteredSnail = ExitedArea.Owner as Snail;
			if (EnteredSnail != null)
			{
				EnteredSnail.CanMove = true;
				GD.Print("Exit");
			}
			EmitSignal("SnailExited");
		}
	}
}
