using Godot;
using System;

public partial class Obstacle : Area2D
{
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			// Llamamos al método Hit() del jugador
			if (body.HasMethod("Hit"))
			{
				body.Call("Hit");
			}

			//Más adelante puedes hacer algo así:
			// if (Name.Contains("Lava")) { /* daño continuo */ }
			// else if (Name.Contains("Spike")) { /* empujón */ }
		}
	}
}
