using Godot;
using System;

public partial class Jugador : Node
{
	public string name { get; set; }
	public int hp { get; set; }
	public int armor { get; set; }
	
	public Jugador(string Name, int Hp, int Armor)
	{
		name = Name;
		hp = Hp;
		armor = Armor;
	}
}
