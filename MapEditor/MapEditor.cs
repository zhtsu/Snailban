using Godot;
using System;

public partial class MapEditor : CanvasLayer
{
	private Window ElementBrowerWindow;
	private Window SimulationWindow;
	private ElementBrowser MyElementBrowser;
	private Level SimulationLevel;
	private GridContainer Grid;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ElementBrowerWindow = GetNode<Window>("ElementBrowserWindow");
		SimulationWindow = GetNode<Window>("SimulationWindow");
		MyElementBrowser = GetNode<ElementBrowser>("ElementBrowserWindow/ElementBrowser");
		Grid = GetNode<GridContainer>("Grid");

		SimulationWindow.Connect("close_requested", Callable.From(() => StopSimulate()));
		MyElementBrowser.Connect("SaveAsClicked", Callable.From(() => SaveAs()));
		MyElementBrowser.Connect("SimulateClicked", Callable.From(() => Simulate()));

		InitPanel();
	}

	private void SaveAs()
	{

	}

	private void Simulate()
	{
		PackedScene LevelScene = (PackedScene)GD.Load("res://Scenes/Game/Level.tscn");
		SimulationLevel = (Level)LevelScene.Instantiate();
		SimulationLevel.SimulationMode = true;
		SimulationLevel.MapId = 1;
		SimulationWindow.AddChild(SimulationLevel);
		SimulationWindow.Show();
	}

	private void StopSimulate()
	{
		SimulationWindow.Hide();
		SimulationWindow.RemoveChild(SimulationLevel);
		SimulationLevel.QueueFree();
	}

	private void InitPanel()
	{
		PackedScene ElementButtonScene = (PackedScene)GD.Load("res://MapEditor/ElementButton.tscn");
		for (int i = 0; i < 64; i++)
		{
			ElementButton MyElementButton = (ElementButton)ElementButtonScene.Instantiate();
			Grid.AddChild(MyElementButton);
		}
	}
}
