using Godot;
using System;
using System.Collections;

public partial class View : CanvasLayer
{
	[Signal]
    public delegate void ClosedEventHandler();

	private CustomSignals MySignals;

	public override void _Ready()
	{
		MySignals = GetNode<CustomSignals>("/root/CustomSignals");
		MySignals.SpaceKey += SpaceKeyDown;
	}

    public override void _ExitTree()
    {
		MySignals.SpaceKey -= SpaceKeyDown;
    }

	private void SpaceKeyDown()
	{
		EmitSignal("Closed");
		QueueFree();
	}
}
