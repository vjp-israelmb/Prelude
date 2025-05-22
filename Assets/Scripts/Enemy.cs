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
			var data = GetNodeOrNull<DataCarrier>("/root/DataCarrier");
			if (data != null)
			{
				if (data.nivel == 2)
				{
					// Obtener el nodo padre y hacerle cast a tipo Main
					var mainNode = GetTree().Root.GetNodeOrNull<Main2>("Main");
					if (mainNode is Main2 main)
					{
						main.StartCombat();
					}
					else
					{
						GD.PrintErr("No se pudo obtener el nodo Main desde Enemy.");
					}
				} else {
					// Obtener el nodo padre y hacerle cast a tipo Main
					var mainNode = GetTree().Root.GetNodeOrNull<Main>("Main");
					if (mainNode is Main main)
					{
						main.StartCombat();
					}
					else
					{
						GD.PrintErr("No se pudo obtener el nodo Main desde Enemy.");
					}
				}
			}
			else
			{
				GD.Print("No se encontró DataCarrier");
			}
		}
	}
}
