using Godot;
using System;
using System.Collections.Generic;

public partial class CombatManager : Node
{
	private Node2D backgroundContainer;
	private HBoxContainer handContainer;
	private CardLoader loader;
	private List<Card> deck = new List<Card>();
	private List<Card> hand = new List<Card>();
	
	//Para obtener la animacion del corazon
	private Node2D heartBar;
	private AnimatedSprite2D heart;
	private int playerHealth;
	private int enemyHealth = 100;

	public override void _Ready()
	{
		var player = (Player)GetNode("/root/Player");
		// Instanciar el fondo
		backgroundContainer = GetNode<Node2D>("BgContainer");
		handContainer = GetNode<HBoxContainer>("CombatUI/Mano");
  		
		// Instanciar el loader de cartas
		loader = new CardLoader();
		// Cargar el mazo y repartir cartas
		LoadDeck();
		DrawHand(6);
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
				playerHealth += card.LevelEffect;
			GD.Print($"El jugador se curó {card.LevelEffect}. Salud del jugador: {playerHealth}");
		}

		if (card.Type == "armor" && card.LevelEffect > 0)
		{
			playerHealth += card.Quantity;
			GD.Print($"El jugador ganó {card.LevelEffect} de armadura. Salud del jugador: {playerHealth}");
		}

		card.LevelEffect -= card.LevelEffect;
		UpdateUI();
	}

	private void UpdateUI()
	{
		GD.Print($"Salud del jugador: {playerHealth}, Salud del enemigo: {enemyHealth}");
	}

	private void EndCombat()
	{
		// Eliminar todos los botones hijos de handContainer
		foreach (Node child in handContainer.GetChildren())
		{
			child.QueueFree();
		}

		// Cambiar de escena al juego principal
		GetTree().ChangeSceneToFile("res://scenes/main.tscn");
	}
}
