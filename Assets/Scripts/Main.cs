using Godot;
using System;

public partial class Main : Node
{
	// Posiciones iniciales del jugador y la cámara
	public static readonly Vector2 PLAYER_START_POS = new Vector2(-514,-167); //posicion inicial del jugador
	public static readonly Vector2 CAM_START_OFFSET = new Vector2(0, -255); // Ajuste relativo al jugador
	public static  Vector2 screen_size= new Vector2(0, -255);

	// Velocidades
	private float speed;
	public const float START_SPEED = 400.0f;
	public const float MAX_SPEED = 650.0f;

	// Referencias a los nodos
	private CharacterBody2D player;
	private Camera2D camera;
	private StaticBody2D ground;

	public override void _Ready()
	{	//Obtener tamaño de la pantalla
		screen_size=DisplayServer.WindowGetSize();
		
		// Obtener referencias a los nodos
		player = GetNode<CharacterBody2D>("Player");
		camera = GetNode<Camera2D>("Camera2D");
		ground = GetNode<StaticBody2D>("Ground");
		
		NewGame();
	}

	public void NewGame()
	{
		// Posicionar al jugador en su punto de inicio
		player.Position = PLAYER_START_POS;
		player.Velocity = Vector2.Zero;

		// 🔹 Posicionar la cámara RELATIVAMENTE al jugador
		camera.Position =CAM_START_OFFSET;
		ground.Position = Vector2.Zero;
	}

	  public override void _Process(double delta)
	{
		speed = START_SPEED;

		// Mover player y cámara en X
		player.Position += new Vector2(speed * (float)delta, 0);
		camera.Position += new Vector2(speed * (float)delta, 0);
		
		// generacion de suelo unico de manera indefinida
		if (camera.Position.X - ground.Position.X > screen_size.X * 1.3){
			 GD.Print("Reposicionando el suelo");
			//Creo una variable nueva y se lo asigno al suelo actual 
			Vector2 newGroundPos = ground.Position;
			newGroundPos.X += screen_size.X;
			ground.Position = newGroundPos;
		}
	}
}
