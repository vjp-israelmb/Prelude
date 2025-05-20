using Godot;
using System;
using System.Collections.Generic;

public partial class CombatManager : Node2D
{
	// Precarga Enemigos
	private PackedScene frogrosso = GD.Load<PackedScene>("res://Assets/Prefabs/frogrossoDisplay.tscn");
	private PackedScene eagearl = GD.Load<PackedScene>("res://Assets/Prefabs/eagearl.tscn");
	private PackedScene[] enemies;
	
	private Node2D backgroundContainer;
	private HBoxContainer handContainer;
	private List<Card> hand = new List<Card>();
	public bool Visible = false;
	
	//Para obtener la animacion del corazon
	private Node2D heartBar;
	private AnimatedSprite2D heart;
	private Jugador player;
	private Jugador enemy;

	public void inicioCombate(Jugador Player, Jugador Enemy)
	{
		var mainNode = GetTree().Root.GetNodeOrNull<Main>("Main");
		if(mainNode.isInCombat)
		{
			return;
		}else
		{
			mainNode.isInCombat = true;
		}
		
		enemies = new PackedScene[] { frogrosso, eagearl};
		handContainer = GetNode<HBoxContainer>("CombatUI/Mano");
		
		player = Player;
		hand = player.mano;
		enemy = Enemy;
		GD.Print("Instanciando enemigo: ", enemy.name);
		
		int i = 0;
		if (enemy.name.ToLower().Contains("eagearl"))
		{
			i = 1;
		} else if (enemy.name.ToLower().Contains("frogrosso"))
		{
			i = 0;
		} else if (enemy.name.ToLower().Contains("mindy"))
		{
			i = 2;
		} else
		{
			i = 3;
		}
		
		// Instanciar el enemigo
		var scene = enemies[i];
		var enemyScene = scene.Instantiate<Node2D>();
		var camera = mainNode.GetNode<Camera2D>("Camera2D");
		var ground = mainNode.GetNode<StaticBody2D>("Ground");
		float spawnX = camera.Position.X + Main.screen_size.X;
		float spawnY = ground.Position.Y - 100;
		enemyScene.Position = camera.Position;
		enemyScene.Name = "enemy";
		AddChild(enemyScene);
		GD.Print("Escena instanciada: ", scene);
		
		cagarMano();
	}

	private void cagarMano()
	{
		int totalCartas = hand.Count;

		for (int i = 0; i < totalCartas; i++)
		{
			Card card = hand[i];

			// Instanciar el botón de la carta
			var buttonScene = GD.Load<PackedScene>(cargarEscena(card.Name));
			var cardButton = buttonScene.Instantiate<CardButton>();
			cardButton.SetCard(card);

			cardButton.Pressed += () => OnCardPressed(card);
			handContainer.AddChild(cardButton);
		}
	}
	
	private String cargarEscena(String name)
	{
		String ruta;
		switch(name)
		{
			case "BoardCastle": 
				ruta = "res://Assets/Scenes/CardHobbo/CardButtonBoardCastle.tscn";
				return ruta;
			case "BoardCrisis": 
				ruta = "res://Assets/Scenes/CardHobbo/CardButtonBoardCrisis.tscn";
				return ruta;
			case "DeadPigeon": 
				ruta = "res://Assets/Scenes/CardHobbo/CardButtonDeadPigeon.tscn";
				return ruta;
			case "GimmeCoin": 
				ruta = "res://Assets/Scenes/CardHobbo/CardButtonGimmeCoin.tscn";
				return ruta;
			case "LeftOvers": 
				ruta = "res://Assets/Scenes/CardHobbo/CardButtonLeftOvers.tscn";
				return ruta;
			case "Leprosy": 
				ruta = "res://Assets/Scenes/CardHobbo/CardButtonLeprosy.tscn";
				return ruta;
			case "Pickpocket": 
				ruta = "res://Assets/Scenes/CardHobbo/CardButtonPickpocket.tscn";
				return ruta;
			case "SpiritBlast": 
				ruta = "res://Assets/Scenes/CardHobbo/CardButtonSpiritBlast.tscn";
				return ruta;
			case "ArmoredShield": 
				ruta = "res://Assets/Scenes/CardKnight/CardButtonArmoredShield.tscn";
				return ruta;
			case "Bandage": 
				ruta = "res://Assets/Scenes/CardKnight/CardButtonBandage.tscn";
				return ruta;
			case "BladeKiss": 
				ruta = "res://Assets/Scenes/CardKnight/CardButtonBladeKiss.tscn";
				return ruta;
			case "RaiseShields": 
				ruta = "res://Assets/Scenes/CardKnight/CardButtonRaiseShields.tscn";
				return ruta;
			case "Reforged": 
				ruta = "res://Assets/Scenes/CardKnight/CardButtonReforged.tscn";
				return ruta;
			case "Whack": 
				ruta = "res://Assets/Scenes/CardKnight/CardButtonWhack.tscn";
				return ruta;
			default:
				GD.Print("Error: No se encontro la escena.");
				break;
		}
		return null;
	}

	private void OnCardPressed(Card card)
	{
		GD.Print($"Carta jugada: {card.Name}");

		ApplyCardEffect(card);

		// Eliminar la carta de la UI
		if (card.LevelEffect <= 0)
		{
			for (int i = 0; i < handContainer.GetChildCount(); i++)
			{
				var child = handContainer.GetChild(i);
				if (child is CardButton cb && cb.GetCard() == card)
				{
					cb.QueueFree();
					break;
				}
			}
			hand.Remove(card);
		}
		GD.Print("Cartas: " + hand.Count);
		if(hand.Count == 0 || player.hp <= 0)
		{
			player.hp = 0;
			EndCombat();
		} else if(enemy.hp <= 0 )
		{
			EndCombat();
		}
	}

	private void ApplyCardEffect(Card card)
	{
		if (card.Type == "damage" && card.LevelEffect > 0)
		{
			if(enemy.armor > 0)
			{
				enemy.armor -= card.Quantity;
				if(enemy.armor < 0)
				{
					enemy.armor = 0;
				}
				GD.Print($"Has realizado {card.Quantity} de daño. Armadura restante del enemigo: {enemy.armor}");
			} else
			{
				enemy.hp -= card.Quantity;
				GD.Print($"Has realizado {card.Quantity} de daño. Salud restante del enemigo: {enemy.hp}");
			}
		}

		if (card.Type == "heal" && card.LevelEffect > 0)
		{
			player.hp += card.Quantity;
			GD.Print($"El jugador se curó {card.LevelEffect}. Salud del jugador: {player.hp}");
		}

		if (card.Type == "armor" && card.LevelEffect > 0)
		{
			player.armor += card.Quantity;
			GD.Print($"El jugador ganó {card.LevelEffect} de armadura. Armadura del jugador: {player.armor}");
		}

		card.LevelEffect -= 1;
		GD.Print(card.LevelEffect);
		UpdateUI();
		turnoEnemigo();
	}

	private void UpdateUI()
	{
		GD.Print($"Jugador--> Salud: {player.hp} Armadura: {player.armor}, Enemigo--> Salud: {enemy.hp}, Armor: {enemy.armor}");
	}

	private void turnoEnemigo()
	{
		int index = (int)GD.RandRange(0, enemy.mano.Count - 1);
		GD.Print(enemy.mano.Count);
		Card card = enemy.mano[index];
		
		GD.Print($"Carta jugada: {card.Name}");
		if (card.Type == "damage")
		{
			if(player.armor > 0)
			{
				player.armor -= card.Quantity;
				if(player.armor < 0)
				{
					player.armor = 0;
				}
				GD.Print($"Has recibido {card.Quantity} de daño. Armadura restante: {player.armor}");
			} else
			{
				player.hp -= card.Quantity;
				GD.Print($"Has recibido {card.Quantity} de daño. Salud restante: {player.hp}");
			}
		}

		if (card.Type == "heal")
		{
			enemy.hp += card.Quantity;
			GD.Print($"El enemigo se curó {card.Quantity}. Salud del enemigo: {enemy.hp}");
		}

		if (card.Type == "armor")
		{
			enemy.armor += card.Quantity;
			GD.Print($"El enemigo ganó {card.Quantity} de armadura. Armadura del enemigo: {enemy.armor}");
		}
		UpdateUI();
	}

	private void EndCombat()
	{
		// Eliminar todos los botones hijos de handContainer
		foreach (Node child in handContainer.GetChildren())
		{
			child.QueueFree();
		}
		var enemyScene = GetNode<Node2D>("enemy");
		enemyScene.QueueFree();
		GD.Print("Fin del combate");
		var mainNode = GetTree().Root.GetNodeOrNull<Main>("Main");
		mainNode.EndCombat(player);
	}
}
