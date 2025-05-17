using Godot;
using System;

public partial class Enemy : Area2D
{
	private String Name;
	private int Hp;
	private int Armor;
	
		// En el enemigo (puede estar en un script del enemigo o del Main)
	private void _on_body_entered(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			GD.Print("¡Jugador chocado con enemigo!");
			// Buscar el nodo padre
			Main main = GetParent() as Main;

			// Si no es el padre directo, intenta buscarlo en el árbol
			if (main == null)
			{
				main = GetTree().Root.GetNodeOrNull<Main>("Main");
			}

			if (main != null)
			{
				main.StartCombat();
			}
			else
			{
				GD.PrintErr("No se pudo encontrar el nodo Main.");
			}
		}
	}
}
