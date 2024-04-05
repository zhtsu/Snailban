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
	private GridContainer Grid;
	private Button SelectedElementButton;
	public ElementBean SelectedElementBean = new ElementBean();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SaveAsButton = GetNode<Button>("VBoxContainer/HBoxContainer/SaveAsButton");
		SimulateButton = GetNode<Button>("VBoxContainer/HBoxContainer/SimulateButton");
		Grid = GetNode<GridContainer>("VBoxContainer/ScrollContainer/Grid");
		SelectedElementButton = GetNode<Button>("VBoxContainer/HBoxContainer/SelectedElementButton");

		SaveAsButton.Connect("button_down", Callable.From(() => { EmitSignal("SaveAsClicked"); }));
		SimulateButton.Connect("button_down", Callable.From(() => { EmitSignal("SimulateClicked"); }));

		PackedScene ElementButtonScene = (PackedScene)GD.Load("res://MapEditor/ElementButton.tscn");
		foreach (int Key in ConfigData.ElementBeanDict.Keys)
		{
			ConfigData.ElementBeanDict.TryGetValue(Key, out ElementBean MyElementBean);
			ElementButton MyElementButton = (ElementButton)ElementButtonScene.Instantiate();
			PackedScene ElementScene = (PackedScene)GD.Load(MyElementBean.Path);
			Element MyElement = (Element)ElementScene.Instantiate();
			MyElementBean.Icon = MyElement.GetNode<Sprite2D>("Body").Texture;
			MyElementButton.Icon = MyElementBean.Icon;
			MyElementButton.MyElementBean = MyElementBean;
			MyElementButton.LeftMouseButtonClicked += (() => SetSelectedElement(MyElementBean));
			Grid.AddChild(MyElementButton);
		}
	}

	private void SetSelectedElement(ElementBean InElementBean)
	{
		SelectedElementBean = InElementBean;
		SelectedElementButton.Icon = InElementBean.Icon;
		if (InElementBean.Name.Contains("TP_"))
        {
            SelectedElementButton.Modulate = new Color("#ffffff66");
        }
		else
		{
			SelectedElementButton.Modulate = new Color("#ffffffff");
		}
	}
}
