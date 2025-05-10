using Godot;
using System;

public partial class Menu : CanvasLayer
{
	// Exporta las áreas para asignarlas desde el editor
	[Export] private Area2D vagabondArea;
	[Export] private Area2D knightArea;
	[Export] private Button startButton;
	
	// Variable estática para guardar la selección
	public static PackedScene SelectedCharacter { get; private set; }
	
	public override void _Ready()
	{
		vagabondArea.MouseEntered += () => Global.SelectedCharacter = GD.Load<PackedScene>("res://Assets/Prefabs/vagabond.tscn");
		knightArea.MouseEntered += () => Global.SelectedCharacter = GD.Load<PackedScene>("res://Assets/Prefabs/knight.tscn");
		startButton.Pressed += OnStartPressed;
		// Selección por defecto de personaje
		SelectCharacter("res://Assets/Prefabs/vagabond.tscn");
	}
	
	private void SelectCharacter(string characterPath)
	{
		SelectedCharacter = GD.Load<PackedScene>(characterPath);
		// Efecto visual al seleccionar
		if (characterPath.Contains("vagabond"))
		{
			GD.Print("vagabond Seleccionado");
			vagabondArea.Modulate = Colors.Green;
			knightArea.Modulate = Colors.White;
		}
		else
		{
			GD.Print("knight Seleccionado");
			knightArea.Modulate = Colors.Green;
			vagabondArea.Modulate = Colors.White;
		}
	}
	private void OnStartPressed()
	{
		GetTree().ChangeSceneToFile("res://Assets/Prefabs/main.tscn");
	}
}
