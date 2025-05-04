using Godot;
using System;

public class CombatManager : Node
{
	// Referencia al contenedor donde instanciar el fondo
	private Node2D backgroundContainer = GetNode<Node2D>("BgContainer");

	// Referencias a la interfaz
	[OnReadyGet("CombatUI/Mano")]
	private HBoxContainer handContainer;
	
	// Mazo de cartas y mano del jugador
	private List<Card> deck = new List<Card>();
	private List<Card> hand = new List<Card>();

	// Salud del jugador y enemigo
	private int playerHealth = 100;
	private int enemyHealth = 100;

	public override void _Ready()
	{
		// Instanciar el fondo
		var background = (PackedScene)GD.Load("res://Prefabs/bg.tscn");
		Node backgroundInstance = background.Instance();
		backgroundContainer.AddChild(backgroundInstance);

		// Cargar el mazo y repartir cartas
		LoadDeck();
		DrawHand(5);
	}

	// Función para cargar las cartas del mazo
	private void LoadDeck()
	{
		allDecks = loader.LoadCardsFromFile();

		// Por ejemplo, cargar las cartas de la baraja 'Vagabundo'
		deck = allDecks["Vagabundo"];
		
		// Mostrar las cartas en la interfaz, repartir cartas, etc.
		GD.Print($"Cartas del Vagabundo: {vagabundoDeck.Count}");
	}

	// Función para repartir cartas
	private void DrawHand(int num)
	{
		hand.Clear();
		handContainer.ClearChildren();

		for (int i = 0; i < num; i++)
		{
			Card card = deck[GD.RandRange(0, deck.Count - 1)];
			hand.Add(card);

			// Instanciar un botón para cada carta
			var button = (Button)GD.Load("res://scenes/CardButton.tscn").Instance();
			var cardButton = button as CardButton;
			cardButton.SetCard(card);
			cardButton.Connect("pressed", this, nameof(OnCardPressed), new Array() { card });
			handContainer.AddChild(cardButton);
		}
	}

	// Lógica cuando el jugador presiona una carta
	private void OnCardPressed(Card card)
	{
		GD.Print($"Carta jugada: {card.Name}");

		// Aplicar el efecto de la carta
		ApplyCardEffect(card);

		// Eliminar la carta de la mano
		handContainer.GetChild(0).QueueFree();  // Eliminar la carta de la UI
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
		// Aquí puedes actualizar las barras de vida o algún otro indicador
		GD.Print($"Salud del jugador: {playerHealth}, Salud del enemigo: {enemyHealth}");
	}
	
	private void EndCombat()
	{
		// Regresar al juego principal
		GetTree().ChangeScene("res://scenes/main.tscn");
	}
}
