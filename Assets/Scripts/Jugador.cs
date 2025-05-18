using Godot;
using System;

public partial class Jugador : Node
{
	public string name { get; set; }
	public int hp { get; set; }
	public int armor { get; set; }
	public Card[] mano { get; set; } = new Card[5];
	
	public Jugador(string Name, int Hp, int Armor)
	{
		name = Name;
		hp = Hp;
		armor = Armor;
	}
	
	public void  setMano()
	{
		
	}
}
