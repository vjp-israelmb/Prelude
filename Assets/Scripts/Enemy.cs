using Godot;
using System;

public partial class Enemy : Area2D
{
		// En el enemigo (puede estar en un script del enemigo o del Main)
	private void _on_body_entered(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			GD.Print("¡Jugador chocado con enemigo!");
			// (body as Player)?.Knockback(); // Llamamos al Knockback del Player
			// Cambiar a la escena combate
			GetTree().ChangeScene("res://scenes/CombatManager.tscn");
		}
	}
}
