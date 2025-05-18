using Godot;
using System;

public partial class GameOver : CanvasLayer
{
		[Export]private Button mainMenuButton;
		[Export] private Button exitButton;
	public override void _Ready()
	{
		mainMenuButton.Pressed +=_on_main_menu_pressed;
		exitButton.Pressed +=_on_exit_pressed;
   	}
	
	private void _on_main_menu_pressed()
	{
		GetTree().ChangeSceneToFile("res://Assets/Prefabs/menu.tscn");
	}
	private void _on_exit_pressed()
	{
		GetTree().Quit();
	}
}
