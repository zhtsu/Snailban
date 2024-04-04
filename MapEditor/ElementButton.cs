using Godot;
using System;

public partial class ElementButton : Control
{
	[Signal]
    public delegate void LeftMouseButtonClickedEventHandler();

	[Signal]
    public delegate void RightMouseButtonClickedEventHandler();

    private Button MyButton;
    public int ElementId = -1;

    public override void _Ready()
    {
        MyButton = GetNode<Button>("Button");
    }

    public void DrawElement(int DrawedElementId)
    {
        //MyButton.Icon = null;
    }

    public void ErasureElement()
    {
        MyButton.Icon = null;
        ElementId = -1;
    }

    public void _OnButtonGuiInput(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMouseButton EventMouseButton && EventMouseButton.Pressed)
		{
			if (EventMouseButton.ButtonIndex == MouseButton.Left)
			{
				EmitSignal("LeftMouseButtonClicked");
			}
			else if (EventMouseButton.ButtonIndex == MouseButton.Right)
			{
				EmitSignal("RightMouseButtonClicked");
                ErasureElement();
			}
		}
    }
}
