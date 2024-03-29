using Godot;
using System;

public partial class DialogBox : CanvasLayer
{
	public enum Speaker
	{
		Left,
		Right
	}

	private TextureRect LeftAvatar;
	private TextureRect RightAvatar;
	private Label WordLabel;
	private AnimationPlayer AnimPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LeftAvatar = GetNode<TextureRect>("Box/LeftAvatar");
		RightAvatar = GetNode<TextureRect>("Box/RightAvatar");
		WordLabel = GetNode<Label>("Box/WordLabel");
		AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		
		AnimPlayer.Play("Enter");
	}

	public void Exit()
	{
		
	}

	public void Speak(Speaker CurrentSpeaker, string Word)
	{
		LeftAvatar.Visible = (CurrentSpeaker == Speaker.Left);
		RightAvatar.Visible = (CurrentSpeaker == Speaker.Right);
		WordLabel.Text = Word;
	}
}
