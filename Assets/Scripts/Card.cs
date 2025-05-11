using Godot;
using System;

public class Card
{
	public string Name;
	public string Effect;
	public int LevelEffect;
	public string Type;
	public int Quantity;
	public int MinLevelRequired;

	public Card(string name, string effect, int levelEffect, string type, int quantity, int minLevelRequired)
	{
		Name = name;
		Effect = effect;
		LevelEffect = levelEffect;
		Type = type;
		Quantity = quantity;
		MinLevelRequired = minLevelRequired;
	}
}
