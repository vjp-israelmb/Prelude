using Godot;
using System;

public partial class Menu : CanvasLayer
{
	// Exporta las áreas para asignarlas desde el editor
	[Export] private Area2D vagabondArea;
	[Export] private Area2D knightArea;
	[Export] private Node2D knightSelected;
	[Export] private Node2D hoboSelected;
	[Export] private Button startButton;
	[Export] private AudioStreamPlayer selection;
	// Variable estática para guardar la selección
	public static PackedScene SelectedCharacter { get; private set; }
	
	public override void _Ready()
{
	GD.Print("Ready");
	
	vagabondArea.MouseEntered += () => SelectCharacter("res://Assets/Prefabs/vagabond.tscn");
	knightArea.MouseEntered += () => SelectCharacter("res://Assets/Prefabs/knight.tscn");
	startButton.Pressed += OnStartPressed;

	SelectCharacter("res://Assets/Prefabs/vagabond.tscn");
}

private void SelectCharacter(string characterPath)
{
	selection.Play();
	GD.Print("Seleccionando: " + characterPath);
	
	//SelectedCharacter = GD.Load<PackedScene>();
	Global.SelectedCharacter = characterPath;

	if (characterPath.Contains("vagabond"))
	{
		GD.Print("vagabond Seleccionado");
		hoboSelected.Visible = true;
		knightSelected.Visible = false;
		Global.namePlayer = "Vagabundo";
	}
	else
	{
		GD.Print("knight Seleccionado");
		hoboSelected.Visible = false;
		knightSelected.Visible = true;
		Global.namePlayer = "Guerrero";
	}
}
	private void OnStartPressed()
	{
		GetTree().ChangeSceneToFile("res://Assets/Prefabs/main.tscn");
	}
}
