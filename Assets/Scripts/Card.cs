using Godot;
using System;

public class Card
{
	public string Name { get; set; }
	public string Effect { get; set; }
	public int LevelEffect { get; set; }
	public string Type { get; set; }
	public int MinLevelRequired { get; set; }

	// Constructor
	public Card(string name, string effect, int levelEffect, string type, int minLevelRequired = 0)
	{
		Name = name;
		Effect = effect;
		LevelEffect = levelEffect;
		Type = type;
		MinLevelRequired = minLevelRequired;
	}
}
