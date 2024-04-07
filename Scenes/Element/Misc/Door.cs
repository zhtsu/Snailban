using Godot;
using System;

public partial class Door : Element
{
	public bool Accept = false;

	public void OpenTheDoor()
	{
		GetNode<Sprite2D>("Body").Texture = (Texture2D)GD.Load("res://Assets/Textures/door.png");
		Accept = true;
	}

	public void CloseTheDoor()
	{
		GetNode<Sprite2D>("Body").Texture = (Texture2D)GD.Load("res://Assets/Textures/door_locked.png");
		Accept = false;
	}
}
