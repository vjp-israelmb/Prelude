using Godot;
using System;
using System.Collections.Generic;

public class CardLoader : Node
{
	// Esta función carga las cartas desde el archivo JSON
	public Dictionary<string, List<Card>> LoadCardsFromFile()
	{
		var json = new JSONParseResult();
		Dictionary<string, List<Card>> cardDecks = new Dictionary<string, List<Card>>();
		
		// Leer el archivo JSON
		var file = new File();
		if (file.FileExists("res://Resources/cards.json"))
		{
			file.Open("res://Resources/cards.json", File.ModeFlags.Read);
			var fileContent = file.GetAsText();
			file.Close();

			// Parsear el archivo JSON
			json = JSON.Parse(fileContent);
			if (json.Result is Godot.Collections.Dictionary jsonObject)
			{
				foreach (KeyValuePair<object, object> deckEntry in jsonObject)
				{
					string deckName = (string)deckEntry.Key;
					var deckData = (Godot.Collections.Array)deckEntry.Value;

					List<Card> deckCards = new List<Card>();
					foreach (Dictionary<string, object> cardData in deckData)
					{
						// Crear una carta a partir de los datos
						Card card = new Card(
							(string)cardData["name"],
							(string)cardData["effect"],
							Convert.ToInt32(cardData["levelEffect"]),
							(string)cardData["type"],
							cardData.ContainsKey("minLevelRequired") ? Convert.ToInt32(cardData["minLevelRequired"]) : 0
						);

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
		else
		{
			GD.PrintErr("El archivo de cartas no existe.");
		}

		return cardDecks;
	}
}
