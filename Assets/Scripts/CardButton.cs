using Godot;
using System;

public partial class CardButton : Button
{
	public Card Card { get; set; }

	public void SetCard(Card card)
	{
		this.Card = card;
		Text = card.Name;  // El texto del botón es el nombre de la carta
	}
}
