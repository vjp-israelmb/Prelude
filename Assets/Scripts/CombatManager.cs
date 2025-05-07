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

	private int playerHealth = 100;
	private int enemyHealth = 100;

	public override void _Ready()
	{
		// Instanciar el fondo
		backgroundContainer = GetNode<Node2D>("BgContainer");
		handContainer = GetNode<HBoxContainer>("CombatUI/Mano");

		// Instanciar el loader de cartas
		loader = new CardLoader();

		// Cargar el mazo y repartir cartas
		LoadDeck();
		DrawHand(5);
	}

	// Función para cargar las cartas del mazo
	private void LoadDeck()
	{
		var allDecks = loader.LoadCardsFromFile();

		// Cargar las cartas de la baraja 'Vagabundo'
		if (allDecks.ContainsKey("Vagabundo"))
		{
			deck = allDecks["Vagabundo"];
		}
		else
		{
			GD.PrintErr("No se encontró el mazo 'Vagabundo' en el archivo JSON.");
		}
	}

	// Función para repartir cartas
	private void DrawHand(int num)
	{
		hand.Clear();
		handContainer.Clear();

		for (int i = 0; i < num; i++)
		{
			if (deck.Count > 0)
			{
				Card card = deck[GD.RandRange(0, deck.Count - 1)];
				hand.Add(card);

				// Instanciar un botón para cada carta
				var button = (Button)GD.Load("res://scenes/CardButton.tscn").Instance();
				var cardButton = button as CardButton;
				cardButton.SetCard(card);
				Array container = new Array(){ card };
				cardButton.Connect("pressed", this, nameof(OnCardPressed), container);
				handContainer.AddChild(cardButton);
			}
		}
	}

	// Lógica cuando el jugador presiona una carta
	private void OnCardPressed(Card card)
	{
		GD.Print($"Carta jugada: {card.Name}");

		// Aplicar el efecto de la carta
		ApplyCardEffect(card);

		// Eliminar la carta de la mano
		if (handContainer.GetChildCount() > 0)
		{
			handContainer.GetChild(0).QueueFree();  // Eliminar la carta de la UI
		}
		hand.Remove(card);
	}

	// Aplicar el efecto de la carta (daño, curación, etc.)
	private void ApplyCardEffect(Card card)
	{
		if (card.Damage > 0)
		{
			enemyHealth -= card.Damage;
			GD.Print($"El enemigo recibió {card.Damage} de daño. Salud enemiga: {enemyHealth}");
		}

		if (card.Heal > 0)
		{
			playerHealth += card.Heal;
			GD.Print($"El jugador se curó {card.Heal}. Salud del jugador: {playerHealth}");
		}

		// Actualizar la interfaz
		UpdateUI();
	}

	// Actualizar la interfaz de salud
	private void UpdateUI()
	{
		// Aquí puedes actualizar las barras de vida o algún otro indicador visual
		GD.Print($"Salud del jugador: {playerHealth}, Salud del enemigo: {enemyHealth}");
	}

	private void EndCombat()
	{
		handContainer.Clear();
		// Regresar al juego principal
		GetTree().ChangeScene("res://scenes/main.tscn");
	}
}
