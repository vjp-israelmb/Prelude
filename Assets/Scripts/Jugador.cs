using Godot;
using System;
using System.Collections.Generic;

public partial class Jugador : Node
{
	private CardLoader loader;
	private List<Card> deck = new List<Card>();
	public List<Card> mano { get; set; } = new List<Card>();
	public string name { get; set; }
	public int hp { get; set; }
	public int armor { get; set; }

	public Jugador(string Name, int Hp, int Armor)
	{
		name = Name;
		hp = Hp;
		armor = Armor;
	}

	public void setMano()
	{
		// Instanciar el loader de cartas
		loader = new CardLoader();
		LoadDeck();
		robarMano();
	}
	
	private void LoadDeck()
	{
		var allDecks = loader.LoadCardsFromFile("res://Assets/Resources/cards.json");

		if (allDecks.ContainsKey(name))
		{
			deck = allDecks[name];
		} else
		{
			GD.PrintErr("No se encontró el mazo en el archivo JSON.");
		}
	}
	
	private void robarMano()
	{
		mano.Clear();

		for (int i = 0; i < 5; i++)
		{
			if (deck.Count > 0)
			{
				int index = (int)GD.RandRange(0, deck.Count - 1);
				Card card = deck[index];
				card.LevelEffect = 3;
				
				mano.Add(card);
			}
		}
	}
}
