using Godot;
using System;

public partial class ControlSignals : Node
{
	[Signal]
    public delegate void UpEventHandler();

	[Signal]
    public delegate void DownEventHandler();

	[Signal]
    public delegate void LeftEventHandler();

	[Signal]
    public delegate void RightEventHandler();

    [Signal]
    public delegate void PauseEventHandler();

    [Signal]
    public delegate void ExitEventHandler();

	[Signal]
    public delegate void ConfirmEventHandler();

	[Signal]
    public delegate void CancelEventHandler();
}
