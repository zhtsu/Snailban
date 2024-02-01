using Godot;
using System;

public partial class Console : Control
{
	// [Signal] public delegate void UpEventHandler();
	// [Signal] public delegate void DownEventHandler();
	// [Signal] public delegate void LeftEventHandler();
	// [Signal] public delegate void RightEventHandler();
	// [Signal] public delegate void ConfirmEventHandler();
	// [Signal] public delegate void CancelEventHandler();

	private TouchScreenButton UpButton, DownButton, LeftButton, RightButton, ConfirmButton, CancelButton;
	private Viewport GameViewport;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// AddUserSignal("UpEventHandler");

		UpButton = GetNode<TouchScreenButton>("UpButton");
		DownButton = GetNode<TouchScreenButton>("DownButton");
		LeftButton = GetNode<TouchScreenButton>("LeftButton");
		RightButton = GetNode<TouchScreenButton>("RightButton");
		ConfirmButton = GetNode<TouchScreenButton>("ConfirmButton");
		CancelButton = GetNode<TouchScreenButton>("CancelButton");
		GameViewport = GetNode<Viewport>("SubViewport/Viewport");

		UpButton.Connect("pressed", new Callable(GameViewport, nameof(GameViewport.UpProcess)));
		DownButton.Connect("pressed", new Callable(GameViewport, nameof(GameViewport.DownProcess)));
		LeftButton.Connect("pressed", new Callable(GameViewport, nameof(GameViewport.LeftProcess)));
		RightButton.Connect("pressed", new Callable(GameViewport, nameof(GameViewport.RightProcess)));
		ConfirmButton.Connect("pressed", new Callable(GameViewport, nameof(GameViewport.ConfirmProcess)));
		CancelButton.Connect("pressed", new Callable(GameViewport, nameof(GameViewport.CancelProcess)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
