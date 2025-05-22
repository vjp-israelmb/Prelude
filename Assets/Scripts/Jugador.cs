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
		}
		else
		{
			GD.PrintErr("No se encontró el mazo en el archivo JSON.");
		}
	}
	
	private void robarMano()
	{
		mano.Clear();

		int cardDamage = 0;
		for (int i = 0; i < 5; i++)
		{
			if (deck.Count > 0)
			{
				int index;
				Card card;
				if (cardDamage < 2)
				{
					do
					{
						index = (int)GD.RandRange(0, deck.Count - 1);
						card = deck[index];
					} while (!card.Type.Contains("damage"));
					cardDamage++;
					card.LevelEffect = 3;
					GD.Print("Meto daño.   " + card.Name);
					mano.Add(card);
				} else {
					index = (int)GD.RandRange(0, deck.Count - 1);
					card = deck[index];
					card.LevelEffect = 3;
					mano.Add(card);
				}
			}
		}
	}
	
	public void rellenarMano(List<Card> Mano)
	{
		mano = Mano;
	}

	public void subirNivelCartas()
	{
		for (int i = 0; i < mano.Count; i++)
		{
			mano[i].LevelEffect += 1;
		}
		GD.Print("Subieron de nivel las cartas.");
	}
}
