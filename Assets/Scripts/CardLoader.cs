using Godot;
using System;
using System.Collections.Generic;

public partial class CardLoader : Node
{
	public Dictionary<string, List<Card>> LoadCardsFromFile(String ruta)
	{
		var cardDecks = new Dictionary<string, List<Card>>();

		if (FileAccess.FileExists(ruta))
		{
			try
			{
				using var file = FileAccess.Open(ruta, FileAccess.ModeFlags.Read);
				string fileContent = file.GetAsText();

				var json = Json.ParseString(fileContent);
				
				if (json.VariantType == Variant.Type.Dictionary)
				{
					var jsonObject = (Godot.Collections.Dictionary)json;

					foreach (var deckEntry in jsonObject)
					{
						string deckName = deckEntry.Key.ToString();

						if (deckEntry.Value.VariantType == Variant.Type.Array)
						{
							var deckData = (Godot.Collections.Array)deckEntry.Value;

							List<Card> deckCards = new List<Card>();
							foreach (Variant cardVariant in deckData)
							{
								if (cardVariant.VariantType == Variant.Type.Dictionary)
								{
									var cardData = (Godot.Collections.Dictionary)cardVariant;

									string cardName = cardData["name"].ToString();
									string cardEffect = cardData["effect"].ToString();
									int levelEffect = StringExtensions.ToInt(cardData["levelEffect"].ToString().Replace(".0", ""));
									string cardType = cardData["type"].ToString();
									int quantity = cardData.ContainsKey("quantity")
										? StringExtensions.ToInt(cardData["quantity"].ToString().Replace(".0", "")) : 0;
									
									Card card = new Card(cardName, cardEffect, levelEffect, cardType, quantity);
									deckCards.Add(card);
								}
							}

							cardDecks[deckName] = deckCards;
						}
					}
				}
				else
				{
					GD.PrintErr("Error: el JSON no tiene un formato válido de diccionario.");
				}
			}
			catch (Exception e)
			{
				GD.PrintErr($"Error al leer o parsear el JSON: {e.Message}");
			}
		}
		else
		{
			GD.PrintErr("El archivo de cartas no existe en la ruta indicada.");
		}

		return cardDecks;
	}
}
