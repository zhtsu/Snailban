using Godot;
using System;


public partial class Map : Node2D
{
	[Export]
	private int Row = 16;

	[Export]
	private int Column = 16;

	[Export]
	private int TileWidth = 16;

	[Export]
	private int TileHeight = 16;

	[Export]
	private Vector2 StartPosition = new Vector2(0, 0);

	private int [,] TileMatrix;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitTileMatrix();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void InitTileMatrix()
	{
		TileMatrix = new int[Row,Column];

		for (int i = 0; i < Row; i++)
		{
			for (int j = 0; j < Column; j++)
			{
				TileMatrix[i,j] = -1;
				GD.Print(TileMatrix[i,j]);
			}
		}
	}

	private void Refresh()
	{

	}
}
