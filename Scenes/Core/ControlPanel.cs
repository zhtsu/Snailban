using Godot;
using System;

public partial class ControlPanel : Control
{
	private ControlSignals MyControlSignals;
	private Button UpButton, DownButton, LeftButton, RightButton;
	private Button ConfirmButton, CancelButton, PauseButton, ExitButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MyControlSignals = GetNode<ControlSignals>("/root/ControlSignals");
		UpButton = GetNode<Button>("UpButton");
		DownButton = GetNode<Button>("DownButton");
		LeftButton = GetNode<Button>("LeftButton");
		RightButton = GetNode<Button>("RightButton");
		ConfirmButton = GetNode<Button>("ConfirmButton");
		CancelButton = GetNode<Button>("CancelButton");
		PauseButton = GetNode<Button>("PauseButton");
		ExitButton = GetNode<Button>("ExitButton");

		UpButton.Connect("pressed", new Callable(this, nameof(UpProcess)));
		DownButton.Connect("pressed", new Callable(this, nameof(DownProcess)));
		LeftButton.Connect("pressed", new Callable(this, nameof(LeftProcess)));
		RightButton.Connect("pressed", new Callable(this, nameof(RightProcess)));
		ConfirmButton.Connect("pressed", new Callable(this, nameof(ConfirmProcess)));
		CancelButton.Connect("pressed", new Callable(this, nameof(CancelProcess)));
		PauseButton.Connect("pressed", new Callable(this, nameof(PauseProcess)));
		ExitButton.Connect("pressed", new Callable(this, nameof(ExitProcess)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void UpProcess() { MyControlSignals.EmitSignal(nameof(ControlSignals.Up)); }
	private void DownProcess() { MyControlSignals.EmitSignal(nameof(ControlSignals.Down)); }
	private void LeftProcess() { MyControlSignals.EmitSignal(nameof(ControlSignals.Left)); }
	private void RightProcess() { MyControlSignals.EmitSignal(nameof(ControlSignals.Right)); }
	private void ConfirmProcess() { MyControlSignals.EmitSignal(nameof(ControlSignals.Confirm)); }
	private void CancelProcess() { MyControlSignals.EmitSignal(nameof(ControlSignals.Cancel)); }
	private void PauseProcess() { MyControlSignals.EmitSignal(nameof(ControlSignals.Pause)); }
	private void ExitProcess() { MyControlSignals.EmitSignal(nameof(ControlSignals.Exit)); }
}
