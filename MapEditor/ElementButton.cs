using Godot;
using System;

public partial class ElementButton : Control
{
	[Signal]
    public delegate void LeftMouseButtonClickedEventHandler();

	[Signal]
    public delegate void RightMouseButtonClickedEventHandler();

    private Button MyButton;
    public FElementBean MyElementBean = new FElementBean();
    public Texture2D Icon = (Texture2D)GD.Load("res://Assets/Textures/snail.png");

    public override void _Ready()
    {
        MyButton = GetNode<Button>("Button");
        MyButton.Icon = Icon;
        if (MyElementBean.Name.Contains("TP_"))
        {
            MyButton.Modulate = new Color("#ffffff66");
        }
    }

    public void DrawElement(FElementBean InElementBean)
    {
        MyElementBean = InElementBean;
        MyButton.Icon = InElementBean.Icon;
        if (MyElementBean.Name.Contains("TP_"))
        {
            MyButton.Modulate = new Color("#ffffff66");
        }
    }

    public void ErasureElement()
    {
        MyButton.Icon = null;
        MyElementBean = new FElementBean();
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
                ErasureElement();
                EmitSignal("RightMouseButtonClicked");
			}
		}
    }
}
