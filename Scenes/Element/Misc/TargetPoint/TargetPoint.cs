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
	private ShaderMaterial ShaderBw;
    
    public override void _Ready()
    {
		Area = GetNode<Area2D>("Area2D");
		Area.Connect("area_entered", new Callable(this, nameof(OnSnailEntered)));
		Area.Connect("area_exited", new Callable(this, nameof(OnSnailExited)));
		ShaderBw = (ShaderMaterial)GD.Load("res://Resources/m_bw.tres");
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
				EnteredSnail.InTargetPoint = true;
				EnteredSnail.GetNode<Sprite2D>("Body").Material = ShaderBw;
				LevelRef.MapMatrix[Location.X, Location.Y] = EnteredSnail;
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
				EnteredSnail.InTargetPoint = false;
				EnteredSnail.GetNode<Sprite2D>("Body").Material = null;
			}
			EmitSignal("SnailExited");
		}
	}
}
