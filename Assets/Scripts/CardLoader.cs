using Godot;
using System;
using System.Collections.Generic;

public class CardLoader : Node
{
	// Esta función carga las cartas desde el archivo JSON
	public Dictionary<string, List<Card>> LoadCardsFromFile()
	{
		var cardDecks = new Dictionary<string, List<Card>>();

		// Leer el archivo JSON
		var file = new File();
		if (file.FileExists("res://Resources/cards.json"))
		{
			try
			{
				file.Open("res://Resources/cards.json", File.ModeFlags.Read);
				var fileContent = file.GetAsText();
				file.Close();

				// Parsear el archivo JSON
				var json = JSON.Parse(fileContent);
				if (json is Godot.Collections.Dictionary jsonObject)
				{
					foreach (KeyValuePair<object, object> deckEntry in jsonObject)
					{
						string deckName = (string)deckEntry.Key;
						var deckData = (Godot.Collections.Array)deckEntry.Value;

						List<Card> deckCards = new List<Card>();
						foreach (Godot.Collections.Dictionary cardData in deckData)
						{
							// Crear una carta a partir de los datos
							string cardName = (string)cardData["name"];
							string cardEffect = (string)cardData["effect"];
							int levelEffect = Convert.ToInt32(cardData["levelEffect"]);
							string cardType = (string)cardData["type"];
							int minLevelRequired = cardData.ContainsKey("minLevelRequired") ? Convert.ToInt32(cardData["minLevelRequired"]) : 0;

							Card card = new Card(cardName, cardEffect, levelEffect, cardType, minLevelRequired);
							deckCards.Add(card);
						}

						cardDecks.Add(deckName, deckCards);
					}
				}
				else
				{
					GD.PrintErr("Error al parsear el JSON.");
				}
			}
			catch (Exception e)
			{
				GD.PrintErr($"Error al leer o parsear el archivo JSON: {e.Message}");
			}
		}
		else
		{
			GD.PrintErr("El archivo de cartas no existe.");
		}

		return cardDecks;
	}
}
