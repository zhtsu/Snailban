using Godot;
using System;

public partial class Main : Node
{
	private CustomSignals MySignals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MySignals = GetNode<CustomSignals>("/root/CustomSignals");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

		if (@event is InputEventKey EventKey)
		{
			if (Input.IsActionJustReleased("Up"))
			{
				MySignals.EmitSignal("UpKey");
			}
			else if (Input.IsActionJustReleased("Down"))
			{
				MySignals.EmitSignal("DownKey");
			}
			else if (Input.IsActionJustReleased("Left"))
			{
				MySignals.EmitSignal("LeftKey");
			}
			else if (Input.IsActionJustReleased("Right"))
			{
				MySignals.EmitSignal("RightKey");
			}
			else if (Input.IsActionJustReleased("Space"))
			{
				MySignals.EmitSignal("SpaceKey");
			}
		}
    }
}
