using Godot;
using System;
using System.Collections.Generic;

public partial class CombatManager : Node2D
{
	// Precarga Enemigos
	private PackedScene frogrosso = GD.Load<PackedScene>("res://Assets/Prefabs/frogrosso.tscn");
	private PackedScene eagearl = GD.Load<PackedScene>("res://Assets/Prefabs/eagearl.tscn");
	private PackedScene[] enemies;
	
	private Node2D backgroundContainer;
	private HBoxContainer handContainer;
	private CardLoader loader;
	private List<Card> deck = new List<Card>();
	private List<Card> hand = new List<Card>();
	public bool Visible = false;
	
	//Para obtener la animacion del corazon
	private Node2D heartBar;
	private AnimatedSprite2D heart;
	private Jugador player;
	private Jugador enemy;
	private int enemyHealth = 100;

	public void inicioCombate(Jugador Player, Jugador Enemy)
	{
		enemies = new PackedScene[] { frogrosso, eagearl};
		handContainer = GetNode<HBoxContainer>("CombatUI/Mano");
		// Instanciar el loader de cartas
		loader = new CardLoader();
		LoadDeck();
		
		player = Player;
		enemy = Enemy;
		
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
		var mainNode = GetTree().Root.GetNodeOrNull<Main>("Main");
		var camera = mainNode.GetNode<Camera2D>("Camera2D");
		var ground = mainNode.GetNode<StaticBody2D>("Ground");
		// Posición: justo fuera de la cámara, a la altura del suelo
		float spawnX = camera.Position.X; // un poquito más lejos
		float spawnY = ground.Position.Y;
		enemyScene.Position = new Vector2(spawnX, spawnY);
		enemyScene.ZIndex = 1;
		enemyScene.Name = "enemy";
		AddChild(enemyScene);
		
		DrawHand(5);
	}

	private void LoadDeck()
	{
		var allDecks = loader.LoadCardsFromFile();

		if (allDecks.ContainsKey("Vagabundo"))
		{
			deck = allDecks["Vagabundo"];
		}
		else
		{
			GD.PrintErr("No se encontró el mazo 'Vagabundo' en el archivo JSON.");
		}
	}

	private void DrawHand(int num)
	{
		hand.Clear();

		for (int i = 0; i < num; i++)
		{
			if (deck.Count > 0)
			{
				int index = (int)GD.RandRange(0, deck.Count - 1);
				Card card = deck[index];

				if (card.Quantity > 0)
				{
					hand.Add(card);
					//card.Quantity--;

					// Instanciar el botón de la carta
					var buttonScene = GD.Load<PackedScene>("res://Assets/Scenes/CardButton.tscn");
					var cardButton = buttonScene.Instantiate<CardButton>();
					cardButton.SetCard(card);

					cardButton.Pressed += () => OnCardPressed(card);

					handContainer.AddChild(cardButton);
				}
			}
		}
	}

	private void OnCardPressed(Card card)
	{
		GD.Print($"Carta jugada: {card.Name}");

		ApplyCardEffect(card);

		// Eliminar la carta de la UI
		if (card.LevelEffect == 0)
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
		if(hand.Count == 0)
		{
			EndCombat();
			GD.Print("Fin del combate");
		}
	}

	private void ApplyCardEffect(Card card)
	{
		if (card.Type == "damage" && card.LevelEffect > 0)
		{
			enemyHealth -= card.Quantity;
			GD.Print($"El enemigo recibió {card.LevelEffect} de daño. Salud enemiga: {enemyHealth}");
		}

		if (card.Type == "heal" && card.LevelEffect > 0)
		{
				player.hp += card.LevelEffect;
			GD.Print($"El jugador se curó {card.LevelEffect}. Salud del jugador: {player.hp}");
		}

		if (card.Type == "armor" && card.LevelEffect > 0)
		{
			player.hp += card.Quantity;
			GD.Print($"El jugador ganó {card.LevelEffect} de armadura. Salud del jugador: {player.hp}");
		}

		card.LevelEffect -= card.LevelEffect;
		UpdateUI();
	}

	private void UpdateUI()
	{
		GD.Print($"Salud del jugador: {player.hp}, Salud del enemigo: {enemyHealth}");
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
		var mainNode = GetTree().Root.GetNodeOrNull<Main>("Main");
		mainNode.EndCombat(player);
	}
}
