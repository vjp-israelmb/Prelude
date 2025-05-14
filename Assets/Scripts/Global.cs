using System;
using Godot;

public partial class Global : Node
{
	// Variable estática para almacenar el personaje seleccionado
	public static PackedScene SelectedCharacter { get; set; }
	public static PackedScene globalHeart { get; set; }
	public static String namePlayer { get; set; }
	
	//Valor por defecto
	public override void _Ready()
	{
		SelectedCharacter = GD.Load<PackedScene>("res://Assets/Prefabs/vagabond.tscn");
		globalHeart= GD.Load<PackedScene>("res://Assets/Prefabs/heart.tscn");
		namePlayer = "Vagabond";
	}
}
