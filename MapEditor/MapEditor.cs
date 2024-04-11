using Godot;
using System;
using System.IO;

public partial class MapEditor : CanvasLayer
{
	private Window ElementBrowerWindow;
	private Window SimulationWindow;
	private ElementBrowser MyElementBrowser;
	private Level SimulationLevel;
	private GridContainer Grid;
	private FileDialog SaveAsFileDialog;
	private Godot.Collections.Array MapMatrix = new Godot.Collections.Array();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ElementBrowerWindow = GetNode<Window>("ElementBrowserWindow");
		SimulationWindow = GetNode<Window>("SimulationWindow");
		MyElementBrowser = GetNode<ElementBrowser>("ElementBrowserWindow/ElementBrowser");
		Grid = GetNode<GridContainer>("Grid");
		SaveAsFileDialog = GetNode<FileDialog>("SaveAsFileDialog");

		SimulationWindow.Connect("close_requested", Callable.From(() => StopSimulate()));
		ElementBrowerWindow.Connect("close_requested", Callable.From(() => { GetTree().Quit(); }));
		MyElementBrowser.Connect("SaveAsClicked", Callable.From(() => SaveAs()));
		MyElementBrowser.Connect("SimulateClicked", Callable.From(() => Simulate()));

		InitPanel();
	}

	private void SaveAs()
	{
		SaveAsFileDialog.Popup();
	}

	private void SaveMapDataToJson(string Path)
	{
		Godot.FileAccess File = Godot.FileAccess.Open(Path, Godot.FileAccess.ModeFlags.Write);
		Godot.Collections.Dictionary MapJson = new Godot.Collections.Dictionary();
		MapJson["id"] = "0";
		MapJson["name"] = "Simulation";
		MapJson["next_level"] = "1";
		MapJson["matrix"] = MapMatrix;
		File.StoreString(MapJson.ToString());
		File.Close();
	}

	private void Simulate()
	{
		PackedScene LevelScene = (PackedScene)GD.Load("res://Scenes/Game/Level.tscn");
		SimulationLevel = (Level)LevelScene.Instantiate();
		SimulationLevel.SimulationMode = true;
		SimulationLevel.MapBean = new FMapBean();
		SimulationLevel.MapBean.Matrix = new int[8,8];
		for (int i = 0; i < 8; i++)
		{
			Godot.Collections.Array Row = (Godot.Collections.Array)MapMatrix[i];
			for (int j = 0; j < 8; j++)
			{
				SimulationLevel.MapBean.Matrix[i,j] = (int)Row[j];
			}
		}
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
		for (int i = 0; i < 8; i++)
		{
			int ii = i;
			Godot.Collections.Array Row = new Godot.Collections.Array();
			for (int j = 0; j < 8; j++)
			{
				int jj = j;
				ElementButton MyElementButton = (ElementButton)ElementButtonScene.Instantiate();
				MyElementButton.Icon = null;
				MyElementButton.LeftMouseButtonClicked += (() => { 
					MyElementButton.DrawElement(MyElementBrowser.SelectedElementBean);
					((Godot.Collections.Array)MapMatrix[ii])[jj] = MyElementBrowser.SelectedElementBean.Id;
				});
				MyElementButton.RightMouseButtonClicked += (() => {
					((Godot.Collections.Array)MapMatrix[ii])[jj] = -1;
				});
				Grid.AddChild(MyElementButton);
				Row.Add(MyElementButton.MyElementBean.Id);
			}
			MapMatrix.Add(Row);
		}
	}
}
