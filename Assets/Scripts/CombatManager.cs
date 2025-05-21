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
			case "CaptivePower": 
				ruta = "res://Assets/Scenes/CardKnight/CardButtonCaptivePower.tscn";
				return ruta;
			case "ForgedWarrior": 
				ruta = "res://Assets/Scenes/CardKnight/CardButtonForgedWarrior.tscn";
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
		if (card.LevelEffect <= 0)
			return;

		switch (card.Type)
		{
			case "damage":
				if (enemy.armor > 0)
				{
					enemy.armor -= card.Quantity;
					if (enemy.armor < 0)
					{
						int remainingDamage = -enemy.armor;
						enemy.armor = 0;
						enemy.hp -= remainingDamage;
						GD.Print($"Has atravesado la armadura y causado {remainingDamage} de daño a la salud. Salud restante: {enemy.hp}");
					}
					else
					{
						GD.Print($"Has hecho {card.Quantity} de daño a la armadura. Armadura restante del enemigo: {enemy.armor}");
					}
				}
				else
				{
					enemy.hp -= card.Quantity;
					GD.Print($"Has hecho {card.Quantity} de daño. Salud restante del enemigo: {enemy.hp}");
				}
				break;
			case "damageArmor":
				enemy.armor -= card.Quantity;
				if (enemy.armor < 0) enemy.armor = 0;
				GD.Print($"Has hecho {card.Quantity} de daño a la armadura. Armadura restante del enemigo: {enemy.armor}");
				break;
			case "heal":
				player.hp += card.Quantity;
				GD.Print($"Te has curado {card.Quantity} de vida. Salud actual: {player.hp}");
				break;
			case "armor":
				player.armor += card.Quantity;
				GD.Print($"Has ganado {card.Quantity} de armadura. Armadura actual: {player.armor}");
				break;
			case "armorHeal":
				player.armor += card.Quantity;
				player.hp += card.Quantity;
				GD.Print($"Has ganado {card.Quantity} de armadura y te has curado {card.Quantity} de vida.");
				break;
			case "armorDraw":
				player.armor += card.Quantity;
				robarCarta();
				GD.Print($"Has ganado {card.Quantity} de armadura y robado una carta.");
				break;
			case "healDraw":
				player.hp += card.Quantity;
				robarCarta();
				GD.Print($"Te has curado {card.Quantity} de vida y robado una carta.");
				break;
			case "draw":
				robarCarta();
				GD.Print("Has robado una carta.");
				break;
			default:
				GD.Print("Tipo de carta no reconocido.");
				break;
		}

		card.LevelEffect -= 1;
		UpdateUI();
		turnoEnemigo();
	}

	private void robarCarta()
	{
		var loader = new CardLoader();
		// Eliminar todos los botones hijos de handContainer
		foreach (Node child in handContainer.GetChildren())
		{
			child.QueueFree();
		}
		var allDecks = loader.LoadCardsFromFile("res://Assets/Resources/cards.json");

		if (allDecks.ContainsKey(player.name))
		{
			var deck = allDecks[player.name];
			if (deck.Count > 0)
			{
				int index = (int)GD.RandRange(0, deck.Count - 1);
				Card card = deck[index];

				if (player.mano.Count < 5)
				{
					player.mano.Add(card);
				}
			}
		}
		else
		{
			GD.PrintErr("No se encontró el mazo en el archivo JSON.");
		}
		
		cagarMano();
	}

	private void UpdateUI()
	{
		GD.Print($"Jugador--> Salud: {player.hp} Armadura: {player.armor}, Enemigo--> Salud: {enemy.hp}, Armor: {enemy.armor}");
	}

	private void turnoEnemigo()
	{
		if (enemy.mano.Count == 0)
		{
			GD.Print("El enemigo no tiene cartas.");
			return;
		}

		int index = (int)GD.RandRange(0, enemy.mano.Count - 1);
		Card card = enemy.mano[index];

		GD.Print($"Carta jugada por el enemigo: {card.Name}");

		if (card.LevelEffect <= 0)
		{
			GD.Print("La carta no tiene efecto restante.");
			return;
		}

		switch (card.Type)
		{
			case "damage":
				if (player.armor > 0)
				{
					player.armor -= card.Quantity;
					if (player.armor < 0)
					{
						int remainingDamage = -player.armor;
						player.armor = 0;
						player.hp -= remainingDamage;
						GD.Print($"La armadura fue superada y recibiste {remainingDamage} de daño. Salud actual: {player.hp}");
					}
					else
					{
						GD.Print($"Recibiste {card.Quantity} de daño a la armadura. Armadura restante: {player.armor}");
					}
				}
				else
				{
					player.hp -= card.Quantity;
					GD.Print($"Recibiste {card.Quantity} de daño. Salud restante: {player.hp}");
				}
				break;

			case "heal":
				enemy.hp += card.Quantity;
				GD.Print($"El enemigo se curó {card.Quantity} de vida. Salud del enemigo: {enemy.hp}");
				break;

			case "armor":
				enemy.armor += card.Quantity;
				GD.Print($"El enemigo ganó {card.Quantity} de armadura. Armadura del enemigo: {enemy.armor}");
				break;

			case "damageArmor":
				player.armor -= card.Quantity;
				if (player.armor < 0)
				{
					player.armor = 0;
				}
				GD.Print($"El enemigo redujo tu armadura en {card.Quantity}. Armadura restante: {player.armor}");
				break;

			default:
				GD.Print("Tipo de carta enemigo no reconocido.");
				break;
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
