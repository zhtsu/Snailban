using Godot;
using System;

public partial class ElementBrowser : CanvasLayer
{
	[Signal]
    public delegate void SaveAsClickedEventHandler();

	[Signal]
    public delegate void SimulateClickedEventHandler();

	private Button SaveAsButton;
	private Button SimulateButton;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SaveAsButton = GetNode<Button>("VBoxContainer/HBoxContainer/SaveAsButton");
		SimulateButton = GetNode<Button>("VBoxContainer/HBoxContainer/SimulateButton");

		SaveAsButton.Connect("button_down", Callable.From(() => { EmitSignal("SaveAsClicked"); }));
		SimulateButton.Connect("button_down", Callable.From(() => { EmitSignal("SimulateClicked"); }));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
