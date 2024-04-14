using Godot;
using System;

public partial class Thanks : CanvasLayer
{
	private CustomSignals MySignals;
	private Main GameMain;

	public override void _Ready()
	{
		MySignals = GetNode<CustomSignals>("/root/CustomSignals");
		GameMain = (Main)GetTree().GetFirstNodeInGroup("Main");
		if (GameMain != null)
		{
			MySignals.SpaceKey += GameMain.BackToMainMenu;
		}
	}

    public override void _ExitTree()
    {
		if (GameMain != null)
		{
			MySignals.SpaceKey -= GameMain.BackToMainMenu;
		}
    }
}
