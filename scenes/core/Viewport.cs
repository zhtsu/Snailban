using Godot;
using System;

public partial class Viewport : Node2D
{
	public Label Label;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Label = GetNode<Label>("Label");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpProcess()
	{
		Label.Text = "UP";
	}

	public void DownProcess()
	{
		Label.Text = "Down";
	}

	public void LeftProcess()
	{
		Label.Text = "Left";
	}

	public void RightProcess()
	{
		Label.Text = "Right";
	}

	public void ConfirmProcess()
	{
		Label.Text = "Confirm";
	}

	public void CancelProcess()
	{
		Label.Text = "Cancel";
	}
}
