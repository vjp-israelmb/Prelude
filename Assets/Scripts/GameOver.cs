using Godot;
using System;

public partial class GameOver : CanvasLayer
{
		[Export]private Button mainMenuButton;
		[Export] private Button exitButton;
		private AnimatedSprite2D animationMainMenuButton;
		private AnimatedSprite2D animationExitButton;
	public override void _Ready()
	{
		animationMainMenuButton=GetNode<AnimatedSprite2D>("Main_Menu/AnimatedSpriteMainMenu");
		animationExitButton=GetNode<AnimatedSprite2D>("Exit/AnimatedSpriteExit");
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
	private void _on_main_menu_mouse_entered()
	{
		animationMainMenuButton.Play("Default");
	}
	private void _on_exit_mouse_entered()
	{
		animationExitButton.Play("Default");
	}
}
