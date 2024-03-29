using Godot;
using System;

public partial class CustomSignals : Node
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

    [Signal]
    public delegate void LevelStartedEventHandler(int MapId);
}
