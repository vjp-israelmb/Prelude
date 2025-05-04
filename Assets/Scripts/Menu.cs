using Godot;
using System;

public partial class Menu : CanvasLayer
{
	public override void _Ready()
	{
		Button startButton = GetNode<Button>("StartButton"); // Ajusta el path
		startButton.Pressed += OnStartButtonPressed;
	}

	private void OnStartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Assets/Prefabs/main.tscn"); // Carga la escena principal
	}
}
