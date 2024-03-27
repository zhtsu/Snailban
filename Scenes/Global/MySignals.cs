using Godot;
using System;

public partial class MySignals : Node
{
	[Signal]
    public delegate void UpKeyEventHandler();

	[Signal]
    public delegate void DownKeyEventHandler();

	[Signal]
    public delegate void LeftKeyEventHandler();

	[Signal]
    public delegate void RightKeyEventHandler();

    [Signal]
    public delegate void SpaceKeyEventHandler();
}
